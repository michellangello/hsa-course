# Handmade CDN

## 📌 Task Description

Create your own CDN for delivering millions of images across the globe.

- Set up 7 containers:
  - `bind` server (custom DNS)
  - Load balancer 1 (`lb1`)
  - Load balancer 2 (`lb2`)
  - Four image nodes (`node1` to `node4`)
- Implement and compare different load balancing approaches (e.g., round-robin, ip_hash, least_conn)
- Implement efficient caching (e.g., via NGINX)
- Measure and analyze performance for each approach
- Summarize the pros and cons of each technique used

## 🔧 Setup Overview
This project includes:
- **BIND9 DNS server (`sameersbn/bind`)**
  - Serves DNS records for the custom domain `prjctr.com`
  - `cdn.prjctr.com` resolves to both `lb1` and `lb2` using round-robin DNS
  - `us.prjctr.com` resolves only to `lb1`, and `eu.prjctr.com` to `lb2`
- **NGINX-based Load Balancers (`lb1`, `lb2`)**
  - Each load balancer proxies requests to its own group of image nodes

- **Image Nodes (`node1`–`node4`)**
  - Serve static image content

- **Client container**
  - For testing DNS and HTTP responses via `curl`, `nslookup`, and `ping`

## Testing CDN

```
docker exec -it cdn_client bash

root@577355dc7141:/app# nslookup cdn.prjctr.com 172.20.0.10
Server:         172.20.0.10
Address:        172.20.0.10#53

Name:   cdn.prjctr.com
Address: 172.20.0.12
Name:   cdn.prjctr.com
Address: 172.20.0.11

root@577355dc7141:/app# nslookup us.prjctr.com 172.20.0.10
Server:         172.20.0.10
Address:        172.20.0.10#53

Name:   us.prjctr.com
Address: 172.20.0.11

root@577355dc7141:/app# nslookup eu.prjctr.com 172.20.0.10
Server:         172.20.0.10
Address:        172.20.0.10#53

Name:   eu.prjctr.com
Address: 172.20.0.12
```

## 🧪 Performance Testing

I used [`siege`](https://www.joedog.org/siege-home/) inside the `cdn_client` container to test CDN performance under different balancing and caching strategies.

Example command:

```bash
docker exec -it cdn_client siege -b -c200 -t15S http://cdn.prjctr.com/images/yosemite.jpg
```

#### Caching ON

```
Transactions:            30795
Availability:           100.00%
Elapsed time:             14.55 secs
Data transferred:     116950.17 MB
Response time:             0.09 secs
Transaction rate:       2116.49 trans/sec
Throughput:             8037.81 MB/sec
Concurrency:             195.43
Successful transactions: 30795
Failed transactions:         0
Longest transaction:        0.86 secs
Shortest transaction:       0.00 secs
```

#### Round-robin (NO caching)

```
Transactions:            11398
Availability:           100.00%
Elapsed time:             14.51 secs
Data transferred:      43286.18 MB
Response time:             0.25 secs
Transaction rate:        785.53 trans/sec
Throughput:             2983.20 MB/sec
Concurrency:             197.14
Successful transactions: 11398
Failed transactions:         0
Longest transaction:        1.09 secs
Shortest transaction:       0.03 secs
```

#### ip_hash (NO caching)

```
Transactions:            10259
Availability:           100.00%
Elapsed time:             14.19 secs
Data transferred:      38960.61 MB
Response time:             0.27 secs
Transaction rate:        722.97 trans/sec
Throughput:             2745.64 MB/sec
Concurrency:             197.06
Successful transactions: 10259
Failed transactions:         0
Longest transaction:        1.53 secs
Shortest transaction:       0.03 secs
```

#### least_conn (NO caching)

```
Transactions:            12567
Availability:           100.00%
Elapsed time:             14.52 secs
Data transferred:      47725.70 MB
Response time:             0.23 secs
Transaction rate:        865.50 trans/sec
Throughput:             3286.89 MB/sec
Concurrency:             195.40
Successful transactions: 12567
Failed transactions:         0
Longest transaction:        0.83 secs
Shortest transaction:       0.01 secs
```
