Docker MySQL master-slave replication 
========================
## Set up MySQL Cluster
- Create 3 docker containers: mysql-m, mysql-s1, mysql-s2
- Setup master slave replication (Master: mysql-m, Slave: mysql-s1, mysql-s2)
- Write script that will frequently write data to database
- Ensure, that replication is working
- Try to turn off mysql-s1 (stop slave), 
- Try to remove a column in  database on slave node (try to delete last column and column from the middle)

## Testing & Results

### Ensure, that replication is working
 
 run `sh write_to_master.sh`, `sh read_from_slave.sh mysql_s1` and `sh read_from_slave.sh mysql_s2` in separate terminals

![master-slave](/19.Replication/resources/master-slave.gif "Master-slave replication")


### Turning off `mysql_s1` and delete `last column` 
![last column delete](/19.Replication/resources/last_column_delete.jpg "Delete last column ")


### Turning off `mysql_s1` and delete `middle column` 
![middle column delete](/19.Replication/resources/middle_column_delete.jpg "Middle column delete")

logs in `mysql_s1` container

```
2025-03-06 17:31:02 2025-03-06T15:31:02.760708Z 67 [ERROR] [MY-013146] [Repl] Replica SQL for channel '': Worker 1 failed executing transaction 'ANONYMOUS' at source log 1.000003, end_log_pos 11676; Column 1 of table 'mydb.code_2' cannot be converted from type 'int' to type 'varchar(1020(bytes) utf8mb4)', Error_code: MY-013146
2025-03-06 17:31:02 2025-03-06T15:31:02.760860Z 66 [Warning] [MY-010584] [Repl] Replica SQL for channel '': ... The replica coordinator and worker threads are stopped, possibly leaving data in inconsistent state. A restart should restore consistency automatically, although using non-transactional storage for data or info tables or DDL queries could lead to problems. In such cases you have to examine your data (see documentation for details). Error_code: MY-001756
```

### Error Code:
- **MY-013146**: This error suggests a data type mismatch between the master and replica databases.
- **MY-001756**: This is a warning that the replica has stopped due to inconsistency and might need a restart.

**The master database has Column 1 as INT, but the replica expects a VARCHAR**. As a result, replication stopped, and the replica is in an inconsistent state.