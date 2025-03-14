using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IMongoClient>(_ =>
{
	var connectionString = builder.Configuration.GetConnectionString("MongoDb")
		?? throw new Exception("MongoDb connection string is missing");

	return new MongoClient(connectionString);
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/employee", (IMongoClient mongoClient) =>
{
	var employee = new EmployeeFaker().Generate();

	var database = mongoClient.GetDatabase("employees");
	var collection = database.GetCollection<Employee>("employees");

	collection.InsertOne(employee);

	return employee;
});

await app.RunAsync();
