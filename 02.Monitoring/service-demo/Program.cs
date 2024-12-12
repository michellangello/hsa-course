using Bogus;
using MongoDB.Bson;
using MongoDB.Driver;
using Elastic.Clients.Elasticsearch;
using service_demo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMongoClient>(sp =>
{
	var connectionString = builder.Configuration.GetConnectionString("MongoDb");
	if (string.IsNullOrEmpty(connectionString))
		throw new Exception("MongoDb connection string is missing");

	return new MongoClient(connectionString);
});


builder.Services.AddSingleton<ElasticsearchClient>(sp =>
{
	var connectionString = builder.Configuration.GetConnectionString("Elasticsearch");
	if (string.IsNullOrEmpty(connectionString))
		throw new Exception("Elasticsearch connection string is missing");

	var settings = new ElasticsearchClientSettings(new Uri(connectionString));
	var client = new ElasticsearchClient(settings);
	return client;
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/healthcheck", () => StatusCodes.Status200OK);

app.MapGet("/mongo/populate", async (IMongoClient mongoClient) =>
{
	var employee = new EmployeeFaker().Generate();

	var database = mongoClient.GetDatabase("employees");
	var collection = database.GetCollection<Employee>("employees");

	await collection.InsertOneAsync(employee);

	return StatusCodes.Status200OK;
});

app.MapGet("/mongo/search", async (IMongoClient mongoClient) =>
{
	var faker = new Faker();
	var randomFirstName = faker.Name.FirstName().Substring(0, 3).ToLower();

	var database = mongoClient.GetDatabase("employees");
	var collection = database.GetCollection<Employee>("employees");

	var filter = Builders<Employee>.Filter.Regex(e => e.FirstName, new BsonRegularExpression(randomFirstName));
	var employees = (await collection.FindAsync(filter)).ToList();

	return employees;
});

app.MapGet("/elastic/populate", async (ElasticsearchClient elasticClient) =>
{
	var employee = new EmployeeFaker().Generate();

	var indexResponse = await elasticClient.IndexAsync(employee,
		i => i.Index("employees"));

	return indexResponse.IsSuccess()
		? StatusCodes.Status200OK
		: StatusCodes.Status500InternalServerError;
});

app.MapGet("/elastic/search", async (ElasticsearchClient elasticClient) =>
{
	var faker = new Faker();
	var randomFirstName = faker.Name.FirstName().Substring(0, 3).ToLower();

	var indexResponse = await elasticClient.SearchAsync<Employee>(s => s
		.Index("employees")
		.From(0)
		.Size(10)
		.Query(q => q
			.Prefix(m => m
				.Field(f => f.FirstName)
				.Value(randomFirstName)
			)
		));


	return indexResponse.IsSuccess()
		? indexResponse.Documents
		: [];
});

app.Run();
