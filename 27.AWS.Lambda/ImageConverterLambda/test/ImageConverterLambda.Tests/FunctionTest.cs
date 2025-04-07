using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.TestUtilities;
using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using Xunit;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageConverterLambda.Tests;

public class FunctionTest
{
    [Fact]
    public async Task TestS3EventLambdaFunction()
    {
        var mockS3Client = new Mock<IAmazonS3>();
        var getObjectResponse = new GetObjectResponse
        {
            ResponseStream = new MemoryStream()
        };

        // Create a simple image and save it to the response stream
        using (var image = new Image<Rgba32>(1, 1))
        {
            await image.SaveAsJpegAsync(getObjectResponse.ResponseStream);
            getObjectResponse.ResponseStream.Position = 0;
        }

        mockS3Client
            .Setup(x => x.GetObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(getObjectResponse));

        mockS3Client
            .Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new PutObjectResponse()));

        // Setup the S3 event object that S3 notifications would create with the fields used by the Lambda function.
        var s3Event = new S3Event
        {
            Records = new List<S3Event.S3EventNotificationRecord>
            {
                new S3Event.S3EventNotificationRecord
                {
                    S3 = new S3Event.S3Entity
                    {
                        Bucket = new S3Event.S3BucketEntity { Name = "s3-bucket" },
                        Object = new S3Event.S3ObjectEntity { Key = "image.jpeg" }
                    }
                }
            }
        };

        // Invoke the lambda function and confirm the content type was returned.
        ILambdaLogger testLambdaLogger = new TestLambdaLogger();
        var testLambdaContext = new TestLambdaContext
        {
            Logger = testLambdaLogger
        };

        var function = new Function(mockS3Client.Object);
        await function.FunctionHandler(s3Event, testLambdaContext);

        // Verify that the PutObjectAsync method was called three times (for BMP, GIF, and PNG)
        mockS3Client.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }
}
