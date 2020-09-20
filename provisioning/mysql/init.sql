
RESET MASTER;
CREATE USER 'cdcuser'@'%' IDENTIFIED BY 'cdcpwd';
-- GRANT REPLICATION SLAVE ON *.* TO 'cdcuser'@'%';
-- GRANT REPLICATION CLIENT ON *.* TO 'cdcuser'@'%';
GRANT ALL PRIVILEGES ON *.* TO 'cdcuser'@'%' ;
FLUSH PRIVILEGES;

DROP DATABASE IF EXISTS test;
CREATE DATABASE IF NOT EXISTS test;
USE test;


CREATE TABLE `test` (
        `id` INT(11) NOT NULL,
        `name` VARCHAR(50) NOT NULL
)