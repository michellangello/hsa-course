#!/bin/bash

# MySQL credentials
DB_USER="root"
DB_PASS="111"
DB_NAME="mydb"
TABLE_NAME="code_2"
CONTAINER_NAME="mysql_m"

# Create table if not exists
docker exec $CONTAINER_NAME sh -c "export MYSQL_PWD=$DB_PASS; mysql -u $DB_USER $DB_NAME -e '
CREATE TABLE IF NOT EXISTS $TABLE_NAME (
    id INT AUTO_INCREMENT PRIMARY KEY,
    code INT,
    description VARCHAR(255)
);'"

while true; do
    # Generate random values
    CODE=$((RANDOM % 1000 + 100))
    DESC="Sample_$CODE"

    # Insert into table
    docker exec $CONTAINER_NAME sh -c "export MYSQL_PWD=$DB_PASS; mysql -u $DB_USER $DB_NAME -e '
    INSERT INTO $TABLE_NAME (code, description) VALUES ($CODE, \"$DESC\");'"

    echo "Inserted: $CODE - $DESC"
    
    # Sleep for 0.5 seconds before the next insert
    sleep 0.5
done