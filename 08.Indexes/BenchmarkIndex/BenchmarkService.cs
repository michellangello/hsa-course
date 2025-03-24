using MySqlConnector;
using System.Diagnostics;

namespace Benchmark;

public class BenchmarkService(string connStr)
{
    public async Task CreateTablesAsync()
    {
        await using var conn = new MySqlConnection(connStr);
        await conn.OpenAsync();

        var cmds = new[]
        {
            "DROP TABLE IF EXISTS users;",
            @"CREATE TABLE IF NOT EXISTS users (
                id BIGINT PRIMARY KEY AUTO_INCREMENT,
                full_name VARCHAR(255),
                date_of_birth DATE
            )  ENGINE=InnoDB;
            
            CREATE INDEX date_of_birth_btree_index USING BTREE ON users (date_of_birth);
            CREATE INDEX date_of_birth_hash_index USING HASH ON users (date_of_birth);
            
            OPTIMIZE TABLE users;"
        };

        foreach (var cmdText in cmds)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = cmdText;
            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task InsertUsersAsync()
    {
        var sw = Stopwatch.StartNew();

        await using var conn = new MySqlConnection(connStr);
        await conn.OpenAsync();

        await using var verifyCmd = conn.CreateCommand();

        verifyCmd.CommandText = "SELECT @@GLOBAL.innodb_flush_log_at_trx_commit;";
        var trxCommit = (ulong)(await verifyCmd.ExecuteScalarAsync() ?? -1);


        foreach (var batch in UserGenerator.GenerateUsers(40_000_000).Chunk(500_000))
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO users (full_name, date_of_birth) VALUES " +
                              string.Join(",", batch.Select((_, i) => $"(@name{i}, @dob{i})"));
            for (int i = 0; i < batch.Length; i++)
            {
                cmd.Parameters.AddWithValue($"@name{i}", batch[i].Name);
                cmd.Parameters.AddWithValue($"@dob{i}", batch[i].Dob);
            }

            await cmd.ExecuteNonQueryAsync();
        }

        sw.Stop();

        Console.WriteLine($"Inserted 40,000,000 users in {sw.Elapsed.TotalSeconds:F2} seconds for innodb_flush_log_at_trx_commit: {trxCommit}");
    }

    public async Task BenchmarkSelectAsync()
    {
        var modes = new (string Label, string IndexHint)[]
        {
            ("btree_index", "USE INDEX (date_of_birth_btree_index)"),
            ("hash_index", "USE INDEX (date_of_birth_hash_index)"),
            ("no_index", "IGNORE INDEX (date_of_birth_btree_index, date_of_birth_hash_index)")
        };

        var dateRanges = new (string Label, string FilterCondition)[]
        {
            ("exact_day", "date_of_birth = '1990-01-15'"),
            ("1_month", "date_of_birth BETWEEN '1990-01-01' AND '1990-02-01'"),
            ("1_year", "date_of_birth BETWEEN '1990-01-01' AND '1991-01-01'"),
            ("5_years", "date_of_birth BETWEEN '1990-01-01' AND '1995-01-01'"),
        };

        foreach (var (label, indexHint) in modes)
        {
            foreach (var (rangeLabel, filter) in dateRanges)
            {
                await using var conn = new MySqlConnection(connStr);
                await conn.OpenAsync();

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = $@"
                    SELECT COUNT(*) 
                    FROM users
                    {indexHint}
                    WHERE {filter};";

                var totalMs = 0L;
                object? result = null;

                for (int i = 0; i < 1; i++)
                {
                    var sw = Stopwatch.StartNew();
                    result = await cmd.ExecuteScalarAsync();
                    sw.Stop();
                    totalMs += sw.ElapsedMilliseconds;
                }

                var avgMs = totalMs;
                Console.WriteLine($"{label,-15} | {rangeLabel,-10} → {avgMs:F2} ms avg ({result} rows)");
            }

            Console.WriteLine();
        }
    }
}
