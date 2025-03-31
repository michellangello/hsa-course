using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLogging();

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
    options.SingleLine = true;
    options.IncludeScopes = false;
});

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(
    "localhost:6379,password=a-very-complex-password-here,ssl=false,abortConnect=false,allowAdmin=true"));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/redis/probabilistic", async (IConnectionMultiplexer redis, ILogger<Program> logger) =>
{
    var db = redis.GetDatabase();
    var cache = new RedisProbabilisticCache(db, logger);

    var randomKey = "probabilistic-key";
    var ttl = TimeSpan.FromSeconds(30);
    var (value, refreshed) = await cache.GetOrCreateAsync(randomKey, HeavyOperation, ttl);

    return Results.Ok(new { Key = randomKey, Value = value, TTL = ttl.TotalSeconds, Refreshed = refreshed });
});

app.Run();

async Task<string> HeavyOperation()
{
    await Task.Delay(2000);
    return Guid.NewGuid().ToString();
}

public class RedisProbabilisticCache(IDatabase db, ILogger logger, double beta = 0.5)
{
    public async Task<(string Value, bool Refreshed)> GetOrCreateAsync(
        string key,
        Func<Task<string>> factory,
        TimeSpan ttl)
    {
        var rawValue = await db.StringGetAsync(key);
        var cached = !rawValue.IsNull
            ? JsonSerializer.Deserialize<CachedValue<string>>(rawValue!)
            : null;

        if (cached != null)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var random = Random.Shared.NextDouble();
            var expiry = cached.Expiry;

            var threshold = now - cached.Delta * beta * Math.Log(random);
            if (threshold < expiry)
                return (cached.Value, false);

            logger.LogInformation($"Cache expired. Current unix time: {now}s, cache expiry unix time: {expiry}s, updated earlier on: {expiry - now}s");
        }
        else
        {
            logger.LogInformation($"Cache miss with key: {key}");
        }


        var payload = await Recompute(factory, ttl);

        var serialized = JsonSerializer.Serialize(payload);

        await db.StringSetAsync(key, serialized, ttl);
        return (payload.Value, true);
    }

    private async Task<CachedValue<T>> Recompute<T>(Func<Task<T>> factory, TimeSpan ttl)
    {
        var sw = Stopwatch.StartNew();

        var newValue = await factory();
        sw.Stop();

        return new CachedValue<T>(newValue)
        {
            Delta = sw.Elapsed.TotalSeconds,
            Expiry = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ttl.TotalSeconds
        };
    }
}

public record CachedValue<T>(T Value)
{
    public required double Expiry { get; init; }
    public required double Delta { get; init; }
}
