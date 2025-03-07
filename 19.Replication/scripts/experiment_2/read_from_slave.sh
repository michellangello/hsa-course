#!/bin/bash

# MySQL credentials
DB_USER="root"
DB_PASS="111"
DB_NAME="mydb"
TABLE_NAME="code_2"
CONTAINER_NAME="$1"

while true; do

   # Retrieve the last inserted row
    LAST_INSERT=$(docker exec $CONTAINER_NAME sh -c "export MYSQL_PWD=$DB_PASS; mysql -u $DB_USER $DB_NAME -e '
    SELECT * FROM $TABLE_NAME ORDER BY id DESC LIMIT 1;'")

    echo "Last inserted row in $1: \n$LAST_INSERT\n"

    # Sleep for 0.5 seconds before the next insert
    sleep 0.5
done