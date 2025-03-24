using Bogus;
using MySqlConnector;

namespace Benchmark;

public class BenchmarkRunner
{
    private const string ConnectionString = "server=localhost;user=benchmark;password=benchmark;database=benchmark_db";

    public async Task RunAsync()
    {
        var service = new BenchmarkService(ConnectionString);

        Console.WriteLine("Creating tables...");
        await service.CreateTablesAsync();

        Console.WriteLine("Generating users...");

        Console.WriteLine("Inserting...");
        await service.InsertUsersAsync();

        Console.WriteLine("Benchmarking...");
        await service.BenchmarkSelectAsync();
    }
}
