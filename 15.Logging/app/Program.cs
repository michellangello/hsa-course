using MySql.Data.MySqlClient;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddTransient<MySqlConnection>(_ => new MySqlConnection(connString));

var app = builder.Build();

app.MapGet("/sleep", async (double? delay, MySqlConnection conn) =>
{
    var finalDelay = delay ?? 0.5;

    await conn.OpenAsync();
    await conn.ExecuteScalarAsync("SELECT SLEEP(@DelayValue);", new { DelayValue = finalDelay }, commandTimeout: 10);
    return $"Done slow query (SLEEP {finalDelay})";
});

app.MapPost("/populate", async (MySqlConnection conn) =>
{
    await conn.OpenAsync();

    // Create table if it does not exist
    var createTableSql = @"
        CREATE TABLE IF NOT EXISTS LargeTable (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            Name VARCHAR(255) NOT NULL,
            Value INT NOT NULL,
            Created DATETIME DEFAULT NOW()
        );
    ";
    await conn.ExecuteAsync(createTableSql);

    // Parameters for population
    int totalRows = 100000;
    int batchSize = 1000;
    for (int i = 0; i < totalRows; i += batchSize)
    {
        var batch = new List<dynamic>();
        for (int j = i; j < i + batchSize && j < totalRows; j++)
        {
            batch.Add(new { Name = $"Item{j}", Value = j });
        }
        var insertSql = "INSERT INTO LargeTable (Name, Value) VALUES (@Name, @Value)";
        await conn.ExecuteAsync(insertSql, batch);
    }
    return $"Inserted {totalRows} rows into LargeTable.";
});

app.MapGet("/query", async (MySqlConnection conn) =>
{
    await conn.OpenAsync();

    // This heavy query retrieves the 1,000 most recent rows
    // and, for each row, calculates:
    //   - ValueCount: how many rows in LargeTable share the same Value.
    //   - ValueAvg: the average Value for that same group.
    var heavyQuery = @"
        SELECT t.*, 
               (SELECT COUNT(*) FROM LargeTable WHERE Value = t.Value) AS ValueCount,
               (SELECT AVG(Value) FROM LargeTable WHERE Value = t.Value) AS ValueAvg
        FROM LargeTable t
        ORDER BY t.Id DESC
        LIMIT 20;
    ";

    var heavyRows = await conn.QueryAsync(heavyQuery);
    return Results.Ok(new { HeavyRows = heavyRows });
});


app.Run();
