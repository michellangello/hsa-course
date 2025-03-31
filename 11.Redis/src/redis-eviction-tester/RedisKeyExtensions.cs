using StackExchange.Redis;

namespace redis_eviction_tester;

public static class RedisKeyExtensions
{
    public static IEnumerable<RedisKey> OrderByName(this IEnumerable<RedisKey> keys) =>
        keys.OrderBy(k => int.Parse(k.ToString().Split('-')[^1]));

    public static IEnumerable<string> FormatWithTtl(this IEnumerable<RedisKey> keys, IDatabase db)
    {
        foreach (var key in keys)
        {
            var ttl = db.KeyTimeToLive(key);
            if (ttl.HasValue)
            {
                yield return $"({key}: ttl {ttl.Value.Seconds}s)";
            }
            else
            {
                yield return $"({key}: no TTL)";
            }
        }
    }
}
