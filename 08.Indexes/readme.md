# InnoDB Index Performance Benchmark

## 📋 Overview

This project benchmarks the performance of MySQL (InnoDB) indexes on a table containing **40 million users**, focusing on queries using the `date_of_birth` column.

### Evaluated Index Types
- ❌ **No Index**
- 🌳 **BTREE Index**
- 🧩 **HASH Index** (Adaptive Hash Index)

Additionally, we evaluate **insert performance** using different `innodb_flush_log_at_trx_commit` values and various batch sizes.

---

## 📊 Benchmark Results

### 🧵 Insert Performance

#### 🔹 40M Users (Batch Size: 100,000)
| `innodb_flush_log_at_trx_commit` | Time (s) |
|----------------------------------|----------|
| 0                                | 243.04   |
| 1                                | 232.61   |
| 2                                | 237.59   |

#### 🔹 100K Users (Batch Size: 1)
| `innodb_flush_log_at_trx_commit` | Time (s) |
|----------------------------------|----------|
| 0                                | 48.95    |
| 1                                | 88.37    |
| 2                                | 53.96    |

✅ **Observation**: For single-row inserts, `innodb_flush_log_at_trx_commit = 1` has the worst performance due to its synchronous flushing behavior. For large batches, differences are minimal.

---

### 🔍 Select Performance

#### Query Types:
- `exact_day`: `date_of_birth = '1990-01-15'`
- `1_month`: `date_of_birth BETWEEN '1990-01-01' AND '1990-02-01'`
- `1_year`: `date_of_birth BETWEEN '1990-01-01' AND '1991-01-01'`
- `5_years`: `date_of_birth BETWEEN '1990-01-01' AND '1995-01-01'`

> ⏱ Average of 10 runs per query

| Index Type   | Query       | Avg Time (ms) |
|--------------|-------------|---------------|
| **BTREE**    | exact_day   | 1.20          |
|              | 1_month     | 11.60         |
|              | 1_year      | 121.80        |
|              | 5_years     | 881.10        |
| **HASH**     | exact_day   | 0.20          |
|              | 1_month     | 10.40         |
|              | 1_year      | 118.60        |
|              | 5_years     | 821.10        |
| **No Index** | exact_day   | 5,325.30      |
|              | 1_month     | 8,460.80      |
|              | 1_year      | 7,751.90      |
|              | 5_years     | 7,451.40      |

✅ **Observation**: 
- **HASH Index** is fastest for equality queries (`exact_day`), leveraging Adaptive Hash Indexing.
- **BTREE Index** is strong for range queries.
- **No Index** performs drastically worse across all queries.

---

## 🧪 Sample SELECT Query

```sql
SELECT COUNT(*) 
FROM users
USE INDEX (date_of_birth_btree_index)
WHERE date_of_birth = '1990-01-15';
```

- **BTREE**: `USE INDEX (date_of_birth_btree_index)`
- **HASH**: `USE INDEX (date_of_birth_hash_index)`
- **No Index**: `IGNORE INDEX (date_of_birth_btree_index, date_of_birth_hash_index)`

---

## 📝 Summary

- **Inserts**:
  - Batch inserts (100K rows) show similar performance regardless of `innodb_flush_log_at_trx_commit` settings.
  - Single inserts (1 row) are significantly slower with `innodb_flush_log_at_trx_commit = 1` due to disk flush overhead.

- **Selects**:
  - **HASH Index (AHI)** is optimal for point lookups (e.g., `WHERE date_of_birth = '...'`).
  - **BTREE Index** handles range queries better.
  - **No Index** leads to extremely poor performance across all query types.

