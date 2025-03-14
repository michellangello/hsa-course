# Load Testing with Siege

## Task Overview

The objective of this task is to create a simple web page that accepts incoming HTTP requests and stores data from each request into a database. We then perform load testing using Siege to evaluate the system's performance under different levels of concurrency.

## Service Overview

The `/employee` endpoint is a POST endpoint that generates a random employee record using the Bogus library and inserts it into a MongoDB database. When a request is made, the service retrieves a database instance named employees, accesses the employees collection, and stores the generated employee data. The response returns the newly created employee record.

## Siege Load Tests

### Concurrency 10

`siege -b -f urls.txt -c10 -t 30s`

```
Transactions:               18058    hits
Availability:                  99.99 %
Elapsed time:                  30.39 secs
Data transferred:               4.09 MB
Response time:                 16.79 ms
Transaction rate:             594.21 trans/sec
Throughput:                     0.13 MB/sec
Concurrency:                    9.98
Successful transactions:    18058
Failed transactions:            1
Longest transaction:          120.00 ms
Shortest transaction:           0.00 ms
```

### Concurrency 25

`siege -b -f urls.txt -c25 -t 30s`

```
Transactions:               19654    hits
Availability:                 100.00 %
Elapsed time:                  30.16 secs
Data transferred:               4.45 MB
Response time:                 38.29 ms
Transaction rate:             651.66 trans/sec
Throughput:                     0.15 MB/sec
Concurrency:                   24.95
Successful transactions:    19654
Failed transactions:            0
Longest transaction:          100.00 ms
Shortest transaction:          20.00 ms
```

### Concurrency 50

`siege -b -f urls.txt -c50 -t 30s`
```
Transactions:               19716    hits
Availability:                 100.00 %
Elapsed time:                  30.15 secs
Data transferred:               4.46 MB
Response time:                 76.33 ms
Transaction rate:             653.93 trans/sec
Throughput:                     0.15 MB/sec
Concurrency:                   49.91
Successful transactions:    19716
Failed transactions:            0
Longest transaction:          350.00 ms
Shortest transaction:          30.00 ms
```

### Concurrency 100

`siege -b -f urls.txt -c100 -t 30s`
```
Transactions:               19844    hits
Availability:                  99.99 %
Elapsed time:                  30.54 secs
Data transferred:               4.49 MB
Response time:                153.39 ms
Transaction rate:             649.77 trans/sec
Throughput:                     0.15 MB/sec
Concurrency:                   99.67
Successful transactions:    19844
Failed transactions:            1
Longest transaction:          330.00 ms
Shortest transaction:          40.00 ms
```

### Concurrency 200

`siege -b -f urls.txt -c200 -t 30s`

```
Transactions:               19343    hits
Availability:                 100.00 %
Elapsed time:                  30.91 secs
Data transferred:               4.38 MB
Response time:                318.23 ms
Transaction rate:             625.78 trans/sec
Throughput:                     0.14 MB/sec
Concurrency:                  199.14
Successful transactions:    19343
Failed transactions:            0
Longest transaction:          740.00 ms
Shortest transaction:          50.00 ms
```

## Summary

-	Availability remains consistently high (≥99.99%) across all tested concurrency levels.
- 	Average response time significantly increases as concurrency grows:
- 	From 16.79 ms at 10 users to 318.23 ms at 200 users.
- 	Throughput stays relatively stable (~0.15 MB/sec), indicating consistent network performance.
- 	Performance degradation at higher concurrency levels suggests potential improvements such as database indexing, caching, and enhanced scalability.

