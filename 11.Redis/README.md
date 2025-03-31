# Redis Cluster 

## Task
 - Build master-slave redis cluster.
 - Try all eviction strategies.
 - Write a wrapper for Redis Client that implement probabilistic cache.

## Docker Compose Structure

The `docker-compose.yml` file is structured to set up a Redis high-availability environment. It consists of:
- **1 Redis Master**: The primary node that handles all write operations.
- **2 Redis Replicas**: A master and a slave for data replication.
- **3 Sentinel Nodes**: For monitoring the master and handling automatic failover.
This setup ensures redundancy and fault tolerance for Redis.

### Check failover mechanism

Try to stop the master node and see how the Sentinel nodes handle the failover process:
```
2025-03-26 15:39:42 1:X 26 Mar 2025 13:39:42.784 * oO0OoO0OoO0Oo Redis is starting oO0OoO0OoO0Oo
2025-03-26 15:39:42 1:X 26 Mar 2025 13:39:42.784 * Redis version=7.4.2, bits=64, commit=00000000, modified=0, pid=1, just started
2025-03-26 15:39:42 1:X 26 Mar 2025 13:39:42.784 * Configuration loaded
2025-03-26 15:39:42 1:X 26 Mar 2025 13:39:42.786 * monotonic clock: POSIX clock_gettime
2025-03-26 15:39:42 1:X 26 Mar 2025 13:39:42.790 * Running mode=sentinel, port=26379.
2025-03-26 15:39:42 1:X 26 Mar 2025 13:39:42.798 * Sentinel new configuration saved on disk
2025-03-26 15:39:42 1:X 26 Mar 2025 13:39:42.798 * Sentinel ID is 5655d1a094a3e976cd25d992197b5142c46cec31
2025-03-26 15:39:42 1:X 26 Mar 2025 13:39:42.798 # +monitor master mymaster 172.22.0.2 6379 quorum 2
2025-03-26 15:39:42 1:X 26 Mar 2025 13:39:42.799 * +slave slave 172.22.0.3:6379 172.22.0.3 6379 @ mymaster 172.22.0.2 6379
2025-03-26 15:39:42 1:X 26 Mar 2025 13:39:42.802 * Sentinel new configuration saved on disk
2025-03-26 15:39:42 1:X 26 Mar 2025 13:39:42.802 * +slave slave 172.22.0.4:6379 172.22.0.4 6379 @ mymaster 172.22.0.2 6379
2025-03-26 15:39:42 1:X 26 Mar 2025 13:39:42.806 * Sentinel new configuration saved on disk
2025-03-26 15:39:44 1:X 26 Mar 2025 13:39:44.791 * +sentinel sentinel a423acaa94cf80e47b3605b24ff007e0123dfe19 172.22.0.6 26379 @ mymaster 172.22.0.2 6379
2025-03-26 15:39:44 1:X 26 Mar 2025 13:39:44.797 * Sentinel new configuration saved on disk
2025-03-26 15:39:44 1:X 26 Mar 2025 13:39:44.822 * +sentinel sentinel fe2ecc01b85119de5b6ed90b2f24d87a494dd631 172.22.0.5 26379 @ mymaster 172.22.0.2 6379
2025-03-26 15:39:44 1:X 26 Mar 2025 13:39:44.827 * Sentinel new configuration saved on disk
2025-03-26 15:42:53 1:X 26 Mar 2025 13:42:53.500 * +fix-slave-config slave 172.22.0.3:6379 172.22.0.3 6379 @ mymaster 172.22.0.2 6379
2025-03-26 15:42:56 1:X 26 Mar 2025 13:42:56.716 # +sdown master mymaster 172.22.0.2 6379
2025-03-26 15:42:56 1:X 26 Mar 2025 13:42:56.796 # +odown master mymaster 172.22.0.2 6379 #quorum 2/2
2025-03-26 15:42:56 1:X 26 Mar 2025 13:42:56.796 # +new-epoch 1
2025-03-26 15:42:56 1:X 26 Mar 2025 13:42:56.796 # +try-failover master mymaster 172.22.0.2 6379
2025-03-26 15:42:56 1:X 26 Mar 2025 13:42:56.800 * Sentinel new configuration saved on disk
2025-03-26 15:42:56 1:X 26 Mar 2025 13:42:56.800 # +vote-for-leader 5655d1a094a3e976cd25d992197b5142c46cec31 1
2025-03-26 15:42:56 1:X 26 Mar 2025 13:42:56.805 * a423acaa94cf80e47b3605b24ff007e0123dfe19 voted for a423acaa94cf80e47b3605b24ff007e0123dfe19 1
2025-03-26 15:42:56 1:X 26 Mar 2025 13:42:56.806 * fe2ecc01b85119de5b6ed90b2f24d87a494dd631 voted for 5655d1a094a3e976cd25d992197b5142c46cec31 1
2025-03-26 15:42:56 1:X 26 Mar 2025 13:42:56.872 # +elected-leader master mymaster 172.22.0.2 6379
2025-03-26 15:42:56 1:X 26 Mar 2025 13:42:56.872 # +failover-state-select-slave master mymaster 172.22.0.2 6379
2025-03-26 15:42:56 1:X 26 Mar 2025 13:42:56.939 # +selected-slave slave 172.22.0.4:6379 172.22.0.4 6379 @ mymaster 172.22.0.2 6379
2025-03-26 15:42:56 1:X 26 Mar 2025 13:42:56.939 * +failover-state-send-slaveof-noone slave 172.22.0.4:6379 172.22.0.4 6379 @ mymaster 172.22.0.2 6379
2025-03-26 15:42:57 1:X 26 Mar 2025 13:42:57.012 * +failover-state-wait-promotion slave 172.22.0.4:6379 172.22.0.4 6379 @ mymaster 172.22.0.2 6379
2025-03-26 15:42:57 1:X 26 Mar 2025 13:42:57.814 * Sentinel new configuration saved on disk
2025-03-26 15:42:57 1:X 26 Mar 2025 13:42:57.814 # +promoted-slave slave 172.22.0.4:6379 172.22.0.4 6379 @ mymaster 172.22.0.2 6379
2025-03-26 15:42:57 1:X 26 Mar 2025 13:42:57.814 # +failover-state-reconf-slaves master mymaster 172.22.0.2 6379
2025-03-26 15:42:57 1:X 26 Mar 2025 13:42:57.882 * +slave-reconf-sent slave 172.22.0.3:6379 172.22.0.3 6379 @ mymaster 172.22.0.2 6379
2025-03-26 15:42:58 1:X 26 Mar 2025 13:42:58.682 * +slave-reconf-inprog slave 172.22.0.3:6379 172.22.0.3 6379 @ mymaster 172.22.0.2 6379
2025-03-26 15:42:58 1:X 26 Mar 2025 13:42:58.682 * +slave-reconf-done slave 172.22.0.3:6379 172.22.0.3 6379 @ mymaster 172.22.0.2 6379
2025-03-26 15:42:58 1:X 26 Mar 2025 13:42:58.741 # +failover-end master mymaster 172.22.0.2 6379
2025-03-26 15:42:58 1:X 26 Mar 2025 13:42:58.742 # +switch-master mymaster 172.22.0.2 6379 172.22.0.4 6379
2025-03-26 15:42:58 1:X 26 Mar 2025 13:42:58.742 * +slave slave 172.22.0.3:6379 172.22.0.3 6379 @ mymaster 172.22.0.4 6379
2025-03-26 15:42:58 1:X 26 Mar 2025 13:42:58.742 * +slave slave 172.22.0.2:6379 172.22.0.2 6379 @ mymaster 172.22.0.4 6379
2025-03-26 15:42:58 1:X 26 Mar 2025 13:42:58.748 * Sentinel new configuration saved on disk
2025-03-26 15:43:03 1:X 26 Mar 2025 13:43:03.815 # +sdown slave 172.22.0.2:6379 172.22.0.2 6379 @ mymaster 172.22.0.4 6379
```


## Redis Eviction Tester

The `redis-eviction-tester` is a tool designed to evaluate the behavior of various Redis eviction policies under memory constraints. It populates the database, triggers evictions, and logs the results, helping analyze how policies like `volatile-lru` and `allkeys-lru` perform.

**=== Testing Eviction Policy: `volatile-lru` ===**
```log
[INFO] Flushing database...
[INFO] Keys count after flush: 0
[INFO] Populating keys (withExpiry=Random)...
[INFO] Keys count after population: 10

[INFO] Performing operations with keys
[INFO] Reading keys 50 times: volatile-lru-0, volatile-lru-1, volatile-lru-2, volatile-lru-3, volatile-lru-4, volatile-lru-5, volatile-lru-6, volatile-lru-7, volatile-lru-8, volatile-lru-9
[INFO] Keys before eviction: (volatile-lru-0: ttl 59s), (volatile-lru-1: no TTL), (volatile-lru-2: no TTL), (volatile-lru-3: no TTL), (volatile-lru-4: ttl 59s), (volatile-lru-5: ttl 59s), (volatile-lru-6: no TTL), (volatile-lru-7: no TTL), (volatile-lru-8: no TTL), (volatile-lru-9: ttl 59s)
[INFO] Adding more keys to trigger eviction...
[INFO] Eviction triggered.

[INFO] Remaining Keys Count: 9
[INFO] Remaining Keys: volatile-lru-1, volatile-lru-2, volatile-lru-3, volatile-lru-6, volatile-lru-7, volatile-lru-8, volatile-lru-17, volatile-lru-18, volatile-lru-19

[INFO] Evicted Keys Count: 4
[INFO] Evicted Keys: volatile-lru-0, volatile-lru-4, volatile-lru-5, volatile-lru-9
```
The above log shows the behavior of the `volatile-lru` eviction policy. The keys with TTL are evicted based on their usage, while the keys without TTL remain in the database. The keys that were least recently used are evicted first.



**=== Testing Eviction Policy: `allkeys-lru` ===**
```log
[INFO] Flushing database...
[INFO] Keys count after flush: 0
[INFO] Populating keys (withExpiry=Random)...
[INFO] Keys count after population: 10

[INFO] Performing operations with keys
[INFO] Reading keys 50 times: allkeys-lru-0, allkeys-lru-1, allkeys-lru-2, allkeys-lru-3, allkeys-lru-4, allkeys-lru-5, allkeys-lru-6
[INFO] Keys before eviction: (allkeys-lru-0: ttl 59s), (allkeys-lru-1: no TTL), (allkeys-lru-2: ttl 59s), (allkeys-lru-3: ttl 59s), (allkeys-lru-4: ttl 59s), (allkeys-lru-5: ttl 59s), (allkeys-lru-6: ttl 59s), (allkeys-lru-7: no TTL), (allkeys-lru-8: no TTL), (allkeys-lru-9: no TTL)
[INFO] Adding more keys to trigger eviction...
[INFO] Eviction triggered.

[INFO] Remaining Keys Count: 11
[INFO] Remaining Keys: allkeys-lru-2, allkeys-lru-7, allkeys-lru-11, allkeys-lru-12, allkeys-lru-13, allkeys-lru-14, allkeys-lru-15, allkeys-lru-16, allkeys-lru-17, allkeys-lru-18, allkeys-lru-19

[INFO] Evicted Keys Count: 8
[INFO] Evicted Keys: allkeys-lru-0, allkeys-lru-1, allkeys-lru-3, allkeys-lru-4, allkeys-lru-5, allkeys-lru-6, allkeys-lru-8, allkeys-lru-9
```
The above log shows the behavior of the `allkeys-lru` eviction policy. All keys are eligible for eviction, regardless of their TTL.


**=== Testing Eviction Policy: `volatile-lfu` ===**
```log
[INFO] Flushing database...
[INFO] Keys count after flush: 0
[INFO] Populating keys (withExpiry=Random)...
[INFO] Keys count after population: 10

[INFO] Performing operations with keys
[INFO] Reading keys 100 times: volatile-lfu-0, volatile-lfu-1, volatile-lfu-2, volatile-lfu-3, volatile-lfu-4, volatile-lfu-5, volatile-lfu-6
[INFO] Keys before eviction: (volatile-lfu-0: ttl 59s), (volatile-lfu-1: no TTL), (volatile-lfu-2: no TTL), (volatile-lfu-3: no TTL), (volatile-lfu-4: ttl 59s), (volatile-lfu-5: ttl 59s), (volatile-lfu-6: no TTL), (volatile-lfu-7: no TTL), (volatile-lfu-8: no TTL), (volatile-lfu-9: no TTL)
[INFO] Adding more keys to trigger eviction...
[INFO] Eviction triggered.

[INFO] Remaining Keys Count: 10
[INFO] Remaining Keys: volatile-lfu-0, volatile-lfu-1, volatile-lfu-2, volatile-lfu-3, volatile-lfu-4, volatile-lfu-6, volatile-lfu-7, volatile-lfu-8, volatile-lfu-9, volatile-lfu-19

[INFO] Evicted Keys Count: 1
[INFO] Evicted Keys: volatile-lfu-5
```
The above log shows the behavior of the `volatile-lfu` eviction policy. The keys with TTL are evicted based on their usage frequency, while the keys without TTL remain in the database. The least frequently used keys are evicted first.

**=== Testing Eviction Policy: `allkeys-lfu` ===**
```log
[INFO] Flushing database...
[INFO] Keys count after flush: 0
[INFO] Populating keys (withExpiry=Random)...
[INFO] Keys count after population: 10

[INFO] Performing operations with keys
[INFO] Reading keys 100 times: allkeys-lfu-0, allkeys-lfu-1, allkeys-lfu-2, allkeys-lfu-3, allkeys-lfu-4, allkeys-lfu-5, allkeys-lfu-6
[INFO] Keys before eviction: (allkeys-lfu-0: ttl 59s), (allkeys-lfu-1: no TTL), (allkeys-lfu-2: ttl 59s), (allkeys-lfu-3: no TTL), (allkeys-lfu-4: ttl 59s), (allkeys-lfu-5: no TTL), (allkeys-lfu-6: ttl 59s), (allkeys-lfu-7: no TTL), (allkeys-lfu-8: no TTL), (allkeys-lfu-9: ttl 59s)
[INFO] Adding more keys to trigger eviction...
[INFO] Eviction triggered.

[INFO] Remaining Keys Count: 11
[INFO] Remaining Keys: allkeys-lfu-0, allkeys-lfu-1, allkeys-lfu-2, allkeys-lfu-3, allkeys-lfu-4, allkeys-lfu-5, allkeys-lfu-6, allkeys-lfu-16, allkeys-lfu-17, allkeys-lfu-18, allkeys-lfu-19

[INFO] Evicted Keys Count: 3
[INFO] Evicted Keys: allkeys-lfu-7, allkeys-lfu-8, allkeys-lfu-9
```
The above log shows the behavior of the `allkeys-lfu` eviction policy. All keys are eligible for eviction, regardless of their TTL. The least frequently used keys are evicted first.


**=== Testing Eviction Policy: `volatile-random` ===**
```log
[INFO] Flushing database...
[INFO] Keys count after flush: 0
[INFO] Populating keys (withExpiry=Random)...
[INFO] Keys count after population: 10

[INFO] Performing operations with keys
[INFO] Keys before eviction: (volatile-random-0: ttl 59s), (volatile-random-1: ttl 59s), (volatile-random-2: ttl 59s), (volatile-random-3: ttl 59s), (volatile-random-4: ttl 59s), (volatile-random-5: no TTL), (volatile-random-6: ttl 59s), (volatile-random-7: no TTL), (volatile-random-8: ttl 59s), (volatile-random-9: no TTL)
[INFO] Adding more keys to trigger eviction...
[INFO] Eviction triggered.

[INFO] Remaining Keys Count: 11
[INFO] Remaining Keys: volatile-random-5, volatile-random-7, volatile-random-8, volatile-random-9, volatile-random-11, volatile-random-13, volatile-random-14, volatile-random-15, volatile-random-16, volatile-random-18, volatile-random-19

[INFO] Evicted Keys Count: 6
[INFO] Evicted Keys: volatile-random-0, volatile-random-1, volatile-random-2, volatile-random-3, volatile-random-4, volatile-random-6
```
The above log shows the behavior of the `volatile-random` eviction policy. The keys with TTL are evicted randomly, while the keys without TTL remain in the database.


**=== Testing Eviction Policy: `allkeys-random` ===**
```log
[INFO] Flushing database...
[INFO] Keys count after flush: 0
[INFO] Populating keys (withExpiry=Random)...
[INFO] Keys count after population: 10

[INFO] Performing operations with keys
[INFO] Keys before eviction: (allkeys-random-0: ttl 59s), (allkeys-random-1: ttl 59s), (allkeys-random-2: ttl 59s), (allkeys-random-3: ttl 59s), (allkeys-random-4: no TTL), (allkeys-random-5: no TTL), (allkeys-random-6: ttl 59s), (allkeys-random-7: no TTL), (allkeys-random-8: ttl 59s), (allkeys-random-9: ttl 59s)
[INFO] Adding more keys to trigger eviction...
[INFO] Eviction triggered.

[INFO] Remaining Keys Count: 11
[INFO] Remaining Keys: allkeys-random-2, allkeys-random-4, allkeys-random-6, allkeys-random-12, allkeys-random-13, allkeys-random-14, allkeys-random-15, allkeys-random-16, allkeys-random-17, allkeys-random-18, allkeys-random-19

[INFO] Evicted Keys Count: 7
[INFO] Evicted Keys: allkeys-random-0, allkeys-random-1, allkeys-random-3, allkeys-random-5, allkeys-random-7, allkeys-random-8, allkeys-random-9
```
The above log shows the behavior of the `allkeys-random` eviction policy. All keys are eligible for eviction, regardless of their TTL. The keys are evicted randomly.


**=== Testing Eviction Policy: `volatile-ttl` ===**
```log
[INFO] Flushing database...
[INFO] Keys count after flush: 0
[INFO] Populating keys (withExpiry=Random)...
[INFO] Keys count after population: 10

[INFO] Performing operations with keys
[INFO] Keys before eviction: (volatile-ttl-0: no TTL), (volatile-ttl-1: ttl 59s), (volatile-ttl-2: ttl 59s), (volatile-ttl-3: no TTL), (volatile-ttl-4: no TTL), (volatile-ttl-5: ttl 59s), (volatile-ttl-6: no TTL), (volatile-ttl-7: no TTL), (volatile-ttl-8: no TTL), (volatile-ttl-9: ttl 59s)
[INFO] Adding more keys to trigger eviction...
[INFO] Eviction triggered.

[INFO] Remaining Keys Count: 11
[INFO] Remaining Keys: volatile-ttl-0, volatile-ttl-3, volatile-ttl-4, volatile-ttl-6, volatile-ttl-7, volatile-ttl-8, volatile-ttl-15, volatile-ttl-16, volatile-ttl-17, volatile-ttl-18, volatile-ttl-19

[INFO] Evicted Keys Count: 4
[INFO] Evicted Keys: volatile-ttl-1, volatile-ttl-2, volatile-ttl-5, volatile-ttl-9
```
The above log shows the behavior of the `volatile-ttl` eviction policy. The keys with TTL are evicted based on their remaining time-to-live, while the keys without TTL remain in the database. The keys with the shortest TTL are evicted first.

**=== Testing Eviction Policy: `noeviction` ===**
```log
[INFO] Flushing database...
[INFO] Keys count after flush: 0
[INFO] Populating keys (withExpiry=Random)...
[INFO] Keys count after population: 10

[INFO] Performing operations with keys
[INFO] Keys before eviction: (noeviction-0: no TTL), (noeviction-1: no TTL), (noeviction-2: no TTL), (noeviction-3: no TTL), (noeviction-4: ttl 59s), (noeviction-5: ttl 59s), (noeviction-6: no TTL), (noeviction-7: no TTL), (noeviction-8: no TTL), (noeviction-9: ttl 59s)
[INFO] Adding more keys to trigger eviction...
StackExchange.Redis.RedisServerException: OOM command not allowed when used memory > 'maxmemory'.
   at Program.<<Main>$>g__TriggerEviction|0_25(IServer server, IDatabase db, String prefix) in /Users/myhailokovalchuk/Courses/Prjct HighLoad/HSA L3/hsa-course/11.Redis/src/redis-eviction-tester/Program.cs:line 132
   at Program.<Main>$(String[] args) in /Users/myhailokovalchuk/Courses/Prjct HighLoad/HSA L3/hsa-course/11.Redis/src/redis-eviction-tester/Program.cs:line 78
```
The above log shows the behavior of the `noeviction` policy. When the memory limit is reached, Redis does not evict any keys, and an error is thrown when trying to add more keys.


## Probabilistic Cache Example

The `probabilistic-example` demonstrates a cache expiration mechanism that uses probabilistic updates to dynamically adjust expiration times. This approach optimizes cache performance by reducing unnecessary cache misses and updates.

Here is a part of the code that shows how the cache expiration is handled:
```c#
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
```

#### run siege

`siege -t1m -c1 http://localhost:5238/redis/probabilistic`

#### output

```
2025-03-31 15:57:46.763 info: Program[0] Cache miss with key: probabilistic-key
2025-03-31 15:58:08.376 info: Program[0] Cache expired. Current unix time: 1743425888s, cache expiry unix time: 1743425898s, updated earlier on: 10s
2025-03-31 15:58:32.723 info: Program[0] Cache expired. Current unix time: 1743425912s, cache expiry unix time: 1743425920s, updated earlier on: 8s
```
As you can see, the cache is expired and updated based on the probabilistic mechanism. The cache miss occurs when the key is not found in the cache, and the cache expiration is logged with the current time and expiry time.

