# Queue Service - Beanstalkd and Redis Integration

## 📌 Overview
This project implements a queue service using **Beanstalkd** and **Redis (RDB & AOF)** for efficient message queuing and processing. The service includes endpoints to **push and pull messages** concurrently.

---

## 🚀 Features
- **Push & Pull APIs** for Beanstalkd, Redis RDB, and Redis AOF
- **Concurrency Handling** to prevent request freezing

---

## 📂 Project Structure
```
.
│-- QueueService/
│   │-- Controllers/
│   │   │-- RedisRdbController.cs
│   │   │-- RedisAofController.cs
│   │   │-- BeanstalkdController.cs
│   │-- Program.cs
│-- docker-compose.yml
│-- telegraf.conf
│-- push.txt
│-- pull.txt
│-- .env
│-- README.md  👈 (this file)
```

---

## 🔥 API Endpoints
### **🔹 Beanstalkd**
- **Push Message:**
  ```http
  POST /beanstalkd/push
  ```
- **Pull Message:**
  ```http
  GET /beanstalkd/pull
  ```

### **🔹 Redis RDB & AOF**
- **Push (RDB):**
  ```http
  POST /redis-rdb/push
  ```
- **Pull (RDB):**
  ```http
  GET /redis-rdb/pull
  ```
- **Push (AOF):**
  ```http
  POST /redis-aof/push
  ```
- **Pull (AOF):**
  ```http
  GET /redis-aof/pull
  ```

---

## 📊 Siege Results

| **Concurrency** | **Operation**       | **Transaction Rate (trans/sec)** | **Throughput (MB/sec)** | **Response Time (ms)** |
|------------------|---------------------|-----------------------------------|--------------------------|-------------------------|
| 50              | RDB Push            | 3902.11                          | 0.00                    | 9.33                   |
| 50              | RDB Pull            | 3771.39                          | 0.11                    | 9.54                   |
| 100             | RDB Push            | 1635.38                          | 0.01                    | 52.19                  |
| 100             | RDB Pull            | 1779.94                          | 0.06                    | 49.13                  |
| 250             | RDB Push            | 206.79                           | 0.02                    | 974.37                 |
| 250             | RDB Pull            | 334.88                           | 0.02                    | 573.11                 |
| 50              | AOF Push            | 5685.62                          | 0.00                    | 8.75                   |
| 50              | AOF Pull            | 5513.43                          | 0.16                    | 9.03                   |
| 100             | AOF Push            | 4854.33                          | 0.00                    | 20.36                  |
| 100             | AOF Pull            | 4737.94                          | 0.14                    | 20.87                  |
| 250             | AOF Push            | 3884.03                          | 0.00                    | 61.95                  |
| 250             | AOF Pull            | 3697.16                          | 0.11                    | 64.88                  |
| 50              | Beanstalk Push      | 4529.03                          | 0.00                    | 7.62                   |
| 50              | Beanstalk Pull      | 18.46                            | 0.00                    | 1653.91                |
| 100             | Beanstalk Push      | 4995.11                          | 0.00                    | 15.88                  |
| 100             | Beanstalk Pull      | 45.87                            | 0.00                    | 2025.54                |

---

## 📌 Summary
- **Redis AOF generally outperforms Redis RDB in push and pull transactions, especially under high concurrency.**
- **Redis RDB performance degrades significantly as concurrency increases, showing high response times at 250 concurrent users.**
- **Beanstalkd has excellent push transaction speed but struggles with pull transactions under load.**
- **For high throughput, AOF is the best choice.**
- **For fast queue insertion, Beanstalkd performs well but may slow down with high pull request loads.**
- **Beanstalkd pull performance suffers because it requires two operations (`reserve` and `delete`) per retrieval, increasing overhead.**
- **Beanstalkd has limited documentation, making troubleshooting and optimizations difficult.**
- **A new connection must be created for each request in Beanstalkd, further impacting performance.**

