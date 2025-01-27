SET autocommit=0;
SET GLOBAL innodb_status_output=ON;
SET GLOBAL innodb_status_output_locks=ON;

CREATE TABLE users
(
	id         INT AUTO_INCREMENT PRIMARY KEY,
	name       VARCHAR(50)  NOT NULL,
	age        INT,
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

INSERT INTO users(name, age) VALUES ("Alice", 20);
INSERT INTO users(name, age) VALUES ("Bob", 25);
select * from users;


commit
