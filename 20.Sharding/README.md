# Sharding

## Task
1. Create 3 docker containers: postgresql-b, postgresql-b1, postgresql-b2
2. Setup horizontal/vertical sharding 
3. Insert 1 000 000 rows into books.
4. Measure performance for reads and writes.
5. Do the same without sharding.
6. Compare performance of 3 cases (without sharding, FDW, and approach of your choice).

## Summary
This document explores different sharding techniques, including Foreign Data Wrapper (FDW) and Citus, and compares their performance against a non-sharded database setup. While sharding can improve scalability, some observed performance decreases may be due to the fact that all database shards are running on the same machine. In a real-world deployment, shards would be distributed across multiple machines, leading to improved performance and fault tolerance.

## API Documentation

### Endpoints

#### Insert Bulk Books
- **URL:** `POST /books/insert-bulk`
- **Description:** Inserts 1,000,000 randomly generated books into the database in batches.
- **Response:**
  - `200 OK`: Successfully inserted 1 million books.

#### Get Random Book Count by Category
- **URL:** `GET /books/random`
- **Description:** Retrieves the count of books in a randomly selected category.
- **Response:**
  - `200 OK`: Returns a JSON object with the count of books and the category ID.

## Configure without sharding

### Setup
```sql
-- Create the books table without sharding
CREATE TABLE IF NOT EXISTS books
(
    id          BIGINT PRIMARY KEY,
    category_id INT  NOT NULL,
    title       TEXT NOT NULL,
    author      TEXT NOT NULL,
    year        INT  NOT NULL
);

-- Create an index on category_id to speed up queries filtering by category
CREATE INDEX books_category_id_idx ON books USING btree(category_id);
```

### Results

#### Insert 1 million rows 
```plaintext
Transactions:            1    hits
Availability:          100.00 %
Elapsed time:            8.97 secs
Data transferred:          0.00 MB
Response time:         8970.00 ms
Transaction rate:          0.11 trans/sec
Throughput:            0.00 MB/sec
Concurrency:            1.00
Successful transactions:        1
Failed transactions:          0
Longest transaction:       8970.00 ms
Shortest transaction:       8970.00 ms
```

#### Random GET 
```plaintext
Transactions:        28332    hits
Availability:           96.50 %
Elapsed time:           30.74 secs
Data transferred:          1.11 MB
Response time:           91.01 ms
Transaction rate:        921.67 trans/sec
Throughput:            0.04 MB/sec
Concurrency:           83.88
Successful transactions:    28332
Failed transactions:       1027
Longest transaction:       1870.00 ms
Shortest transaction:          0.00 ms
```

## Configure sharding with FDW

### Set up tables on each shard

Create table on worker shard node `postgresql-b1`

```sql
-- Create the books table on the first shard
CREATE TABLE IF NOT EXISTS books
(
    id          BIGINT PRIMARY KEY,
    category_id INT  NOT NULL,
    CONSTRAINT category_id_check CHECK ( category_id >= 1 and category_id <= 50 ),
    title       TEXT NOT NULL,
    author      TEXT NOT NULL,
    year        INT  NOT NULL
);

-- Create an index on category_id to optimize queries
CREATE INDEX books_category_id_idx ON books USING btree(category_id);
```

Create table on worker shard node `postgresql-b2`

```sql
-- Create the books table on the second shard
CREATE TABLE IF NOT EXISTS books
(
    id          BIGINT PRIMARY KEY,
    category_id INT  NOT NULL,
    CONSTRAINT category_id_check CHECK ( category_id >= 51 and category_id <= 100),
    title       TEXT NOT NULL,
    author      TEXT NOT NULL,
    year        INT  NOT NULL
);

-- Create an index on category_id to optimize queries
CREATE INDEX books_category_id_idx ON books USING btree(category_id);
```

### Configure FDW on Main DB

Run:

```sql
-- Enable the foreign data wrapper extension
CREATE EXTENSION postgres_fdw;

-- Define foreign servers for both shards
CREATE SERVER books_1_server
FOREIGN DATA WRAPPER postgres_fdw
OPTIONS (host 'postgresql-b1', port '5432', dbname 'books_1');

CREATE USER MAPPING FOR postgres
SERVER books_1_server
OPTIONS (user 'postgres', password 'postgres');

CREATE SERVER books_2_server
FOREIGN DATA WRAPPER postgres_fdw
OPTIONS (host 'postgresql-b2', port '5432', dbname 'books_2');

CREATE USER MAPPING FOR postgres
SERVER books_2_server
OPTIONS (user 'postgres', password 'postgres');

-- Define foreign tables on the main database
CREATE FOREIGN TABLE books_1 (
    id          BIGINT NOT NULL,
    category_id INT  NOT NULL,
    title       TEXT NOT NULL,
    author      TEXT NOT NULL,
    year        INT  NOT NULL
) SERVER books_1_server
OPTIONS (schema_name 'public', table_name 'books');

CREATE FOREIGN TABLE books_2 (
    id          BIGINT NOT NULL,
    category_id INT  NOT NULL,
    title       TEXT NOT NULL,
    author      TEXT NOT NULL,
    year        INT  NOT NULL
) SERVER books_2_server
OPTIONS (schema_name 'public', table_name 'books');

-- Create a unified view of both shards
CREATE VIEW books AS
    SELECT * FROM books_1
    UNION ALL
    SELECT * FROM books_2;

-- Define insert rules to route data to appropriate shards
CREATE RULE books_insert_to_1 AS ON INSERT TO books
WHERE (category_id >= 1 and category_id <= 50)
DO INSTEAD INSERT INTO books_1 VALUES (NEW.*);

CREATE RULE books_insert_to_2 AS ON INSERT TO books
WHERE (category_id >= 51 and category_id <= 100)
DO INSTEAD INSERT INTO books_2 VALUES (NEW.*);

-- Prevent direct modifications on books view
CREATE RULE books_insert AS ON INSERT TO books DO INSTEAD NOTHING;
CREATE RULE books_update AS ON UPDATE TO books DO INSTEAD NOTHING;
CREATE RULE books_delete AS ON DELETE TO books DO INSTEAD NOTHING;
```

## Configure sharding with Citus

### Setup

Set the coordinator

```sql
-- Set the coordinator node
SELECT citus_set_coordinator_host('postgresql-b', 5432);

-- Add worker nodes
SELECT citus_add_node('postgresql-b1', 5432);
SELECT citus_add_node('postgresql-b2', 5432);

-- Create the distributed table
CREATE TABLE IF NOT EXISTS books
(
    id          BIGINT NOT NULL ,
    category_id INT  NOT NULL,
    title       TEXT NOT NULL,
    author      TEXT NOT NULL,
    year        INT  NOT NULL
);

-- Distribute the table across shards based on category_id
SELECT create_distributed_table('books', 'category_id', shard_count => 2);
```

## Sharding Performance Comparison

| Configuration        | Insert Elapsed Time (secs) | Insert Transaction Rate (trans/sec) | Insert Response Time (ms) | Read Transactions | Read Elapsed Time (secs) | Read Response Time (ms) | Read Transaction Rate (trans/sec) | Read Availability (%) | Read Failed Transactions |
| -------------------- | -------------------------- | ----------------------------------- | ------------------------- | ----------------- | ------------------------ | ----------------------- | --------------------------------- | --------------------- | ------------------------ |
| **Without Sharding** | **8.97**                   | **0.11**                            | **8970.00**               | **28332**         | 30.74                    | **91.01**               | **921.67**                        | 96.50                 | 1027                     |
| Sharding with FDW    | 52.25                      | 0.02                                | 52250.00                  | 7724              | 30.58                    | 393.41                  | 252.58                            | 98.38                 | 127                      |
| Sharding with Citus  | 13.94                      | 0.07                                | 13940.00                  | 3026              | **30.23**                | 933.51                  | 100.10                            | **99.51**             | **15**                   |