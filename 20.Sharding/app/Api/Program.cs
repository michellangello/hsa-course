using Bogus;
using Npgsql;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("Postgres")
                       ?? throw new InvalidOperationException("Connection string 'Postgres' not found.");

builder.Services.AddSingleton<NpgsqlDataSource>(_ =>
{
	var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
	return dataSourceBuilder.Build();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("books/insert-bulk", async (NpgsqlDataSource dataSource) =>
	{
		await using var connection = await dataSource.OpenConnectionAsync();
		await using var transaction = await connection.BeginTransactionAsync();

		var faker = new Faker();
		const int batchSize = 500;
		var sb = new StringBuilder();

		for (var i = 0; i < 1_000_000; i++)
		{
			var title = faker.Lorem.Sentence(5).Replace("'", "''");
			var author = faker.Name.FullName().Replace("'", "''");
			var year = Random.Shared.Next(1900, 2026);
			var category = Random.Shared.Next(1, 101);
			var id = Random.Shared.NextInt64();

			sb.Append($"({id}, {category}, '{title}', '{author}', {year}),");

			if (i % batchSize == 0 && i > 0)
			{
				var query =
					$"INSERT INTO books (id, category_id, title, author, year) VALUES {sb.ToString().TrimEnd(',')};";
				await using var command = new NpgsqlCommand(query, connection);
				await command.ExecuteNonQueryAsync();
				sb.Clear();
			}
		}

		if (sb.Length > 0)
		{
			var query =
				$"INSERT INTO books (id, category_id, title, author, year) VALUES {sb.ToString().TrimEnd(',')};";
			await using var command = new NpgsqlCommand(query, connection);
			await command.ExecuteNonQueryAsync();
		}

		await transaction.CommitAsync();

		return Results.Ok("1 million books inserted successfully");
	})
	.WithName("InsertBooks ")
	.WithOpenApi();


app.MapGet("books/random", async (NpgsqlDataSource dataSource) =>
	{
		var category = Random.Shared.Next(1, 101);

		await using var connection = await dataSource.OpenConnectionAsync();

		const string query = @"
	        SELECT count(1) FROM books 
	        WHERE category_id = @category";

		await using var cmd = new NpgsqlCommand(query, connection);
		cmd.Parameters.AddWithValue("@category", category);

		await using var reader = await cmd.ExecuteReaderAsync();
		var countByCategory =
			await reader.ReadAsync() ? reader.GetInt32(0) : 0;
		return Results.Ok(new
		{
			count = countByCategory,
			category = category,
		});
	})
	.WithName("GetRandomBook")
	.WithOpenApi();


app.Run();
