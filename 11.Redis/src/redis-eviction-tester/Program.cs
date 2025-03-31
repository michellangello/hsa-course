using StackExchange.Redis;
using redis_eviction_tester;

var redisInstance = "localhost:6379,password=a-very-complex-password-here,ssl=false,abortConnect=false,allowAdmin=true";
var hostAndPort = "localhost:6379";

Console.WriteLine($"Connecting to Redis instance: {redisInstance}");
var connection = await ConnectionMultiplexer.ConnectAsync(redisInstance);
var server = connection.GetServer(hostAndPort);
var db = connection.GetDatabase();

(string Name, Func<IDatabase, Task> Populate, Func<IDatabase, Task> PerformOperationsWithKeys,
    Func<IServer, IDatabase, Task> TriggerEviction)[] evictionStrategies =
    {
        ("volatile-lru",
            database => PopulateKeys(database, "volatile-lru"),
            database => ReadKeys(database, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 50, "volatile-lru"),
            (srv, database) => TriggerEviction(srv, database, "volatile-lru")),
        ("allkeys-lru",
            database => PopulateKeys(database, "allkeys-lru"),
            database => ReadKeys(database, new[] { 0, 1, 2, 3, 4, 5, 6 }, 50, "allkeys-lru"),
            (srv, database) => TriggerEviction(srv, database, "allkeys-lru")),
        ("volatile-lfu",
            database => PopulateKeys(database, "volatile-lfu"),
            database => ReadKeys(database, new[] { 0, 1, 2, 3, 4, 5, 6 }, 100,
                "volatile-lfu"), // Higher read frequency for LFU
            (srv, database) => TriggerEviction(srv, database, "volatile-lfu")),
        ("allkeys-lfu",
            database => PopulateKeys(database, "allkeys-lfu"),
            database => ReadKeys(database, new[] { 0, 1, 2, 3, 4, 5, 6 }, 100,
                "allkeys-lfu"), // Higher read frequency for LFU
            (srv, database) => TriggerEviction(srv, database, "allkeys-lfu")),
        ("volatile-random",
            database => PopulateKeys(database, "volatile-random"),
            database => Task.CompletedTask, // No specific read pattern for random eviction
            (srv, database) => TriggerEviction(srv, database, "volatile-random")),
        ("allkeys-random",
            database => PopulateKeys(database, "allkeys-random"),
            database => Task.CompletedTask, // No specific read pattern for random eviction
            (srv, database) => TriggerEviction(srv, database, "allkeys-random")),
        ("volatile-ttl",
            database => PopulateKeys(database, "volatile-ttl"),
            database => Task.CompletedTask, // TTL-based eviction doesn't depend on reads
            (srv, database) => TriggerEviction(srv, database, "volatile-ttl")),
        ("noeviction",
            database => PopulateKeys(database, "noeviction"),
            database => Task.CompletedTask, // No eviction occurs
            (srv, database) => TriggerEviction(srv, database, "noeviction"))
    };

Console.WriteLine($"Memory {server.ConfigGet("maxmemory").First()}");

foreach (var policy in evictionStrategies)
{
    try
    {
        Console.WriteLine($"\n**=== Testing Eviction Policy: `{policy.Name}` ===**");
        await server.ConfigSetAsync("maxmemory-policy", policy.Name);

        Console.WriteLine("```log");
        Console.WriteLine($"[INFO] Flushing database...");
        await server.FlushDatabaseAsync();
        var keysAfterFlush = server.Keys(db.Database).ToArray();
        Console.WriteLine($"[INFO] Keys count after flush: {keysAfterFlush.Length}");

        await policy.Populate(db);
        var keysAfterPopulation = server.Keys(db.Database).ToArray();
        Console.WriteLine($"[INFO] Keys count after population: {keysAfterPopulation.Length}");
        Console.WriteLine();

        Console.WriteLine($"[INFO] Performing operations with keys");
        await policy.PerformOperationsWithKeys(db);
        // Capture keys before eviction
        var keysBeforeEviction = server.Keys(db.Database).ToArray();
        Console.WriteLine(
            $"[INFO] Keys before eviction: {string.Join(", ", keysBeforeEviction.OrderByName().FormatWithTtl(db))}");

        await policy.TriggerEviction(server, db);

        // Capture keys after eviction
        var keysAfterEviction = server.Keys(db.Database).ToArray();

        // Determine evicted keys
        var evictedKeys = keysBeforeEviction.Except(keysAfterEviction).ToArray();

        Console.WriteLine();
        Console.WriteLine($"[INFO] Remaining Keys Count: {keysAfterEviction.Length}");
        Console.WriteLine($"[INFO] Remaining Keys: {string.Join(", ", keysAfterEviction.OrderByName())}");
        Console.WriteLine();
        Console.WriteLine($"[INFO] Evicted Keys Count: {evictedKeys.Length}");
        Console.WriteLine($"[INFO] Evicted Keys: {string.Join(", ", evictedKeys.OrderByName())}");
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }

    Console.WriteLine("```");
}

connection.Dispose();
Console.WriteLine("All eviction policies tested.");

// Helper to populate keys
static async Task PopulateKeys(IDatabase db, string prefix, bool? withExpiry = null)
{
    Console.WriteLine($"[INFO] Populating keys (withExpiry={(withExpiry.HasValue ? withExpiry.Value.ToString() : "Random")})...");

    for (var i = 0; i < 10; i++)
    {
        TimeSpan? expiry;
        if (withExpiry.HasValue)
        {
            expiry = withExpiry.Value ? TimeSpan.FromMinutes(i + 1) : null;
        }
        else
        {
            expiry = Random.Shared.NextDouble() >= 0.5 ? TimeSpan.FromMinutes(i + 1) : null;
        }

        await db.StringSetAsync($"{prefix}-{i}", new string('x', 1024 * 20), expiry);
    }
}

// Helper to trigger eviction
static async Task TriggerEviction(IServer server, IDatabase db, string prefix)
{
    Console.WriteLine("[INFO] Adding more keys to trigger eviction...");
    for (var i = 10; i < 20; i++)
    {
        var expiry = TimeSpan.FromMinutes(i + 1);
        await db.StringSetAsync($"{prefix}-{i}", new string('x', 1024 * 20), expiry);
    }

    Console.WriteLine("[INFO] Eviction triggered.");
}

// Helper to read keys
static async Task ReadKeys(IDatabase database, int[] keyIndexes, int times, string prefix)
{
    Console.WriteLine(
        $"[INFO] Reading keys {times} times: {string.Join(", ", keyIndexes.Select(c => $"{prefix}-{c}"))}");
    foreach (var key in keyIndexes)
    {
        for (var i = 0; i < times; i++)
        {
            var result = await database.StringGetAsync($"{prefix}-{key}");
            if (result.IsNull)
                Console.WriteLine($"[INFO] Key {prefix}-{key} not found.");
        }
    }
}
