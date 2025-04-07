using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png;
using System.IO;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ImageConverterLambda
{
    public class Function
    {
        private readonly IAmazonS3 _s3Client;

        // Parameterless constructor
        public Function() : this(new AmazonS3Client()) { }

        public Function(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task FunctionHandler(S3Event evnt, ILambdaContext context)
        {
            foreach (var record in evnt.Records)
            {
                var s3Event = record.S3;
                if (s3Event == null) continue;

                var bucketName = s3Event.Bucket.Name;
                var objectKey = s3Event.Object.Key;

                try
                {
                    using var response = await _s3Client.GetObjectAsync(bucketName, objectKey);
                    await using var responseStream = response.ResponseStream;
                    using var image = await Image.LoadAsync(responseStream);

                    await ConvertAndUploadImage(image, bucketName, objectKey, new BmpEncoder(), "bmp");
                    await ConvertAndUploadImage(image, bucketName, objectKey, new GifEncoder(), "gif");
                    await ConvertAndUploadImage(image, bucketName, objectKey, new PngEncoder(), "png");
                }
                catch (Exception e)
                {
                    context.Logger.LogError($"Error processing object {objectKey} from bucket {bucketName}. Exception: {e.Message}");
                    throw;
                }
            }
        }

        private async Task ConvertAndUploadImage(Image image, string bucketName, string originalKey, IImageEncoder encoder, string extension)
        {
            var newKey = Path.ChangeExtension(originalKey, extension);
            using var memoryStream = new MemoryStream();
            await image.SaveAsync(memoryStream, encoder);
            memoryStream.Position = 0;

            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = newKey,
                InputStream = memoryStream,
                ContentType = $"image/{extension}"
            };

            await _s3Client.PutObjectAsync(putRequest);
        }
    }
}
