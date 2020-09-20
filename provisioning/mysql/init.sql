
RESET MASTER;
SET old_passwords=0;
CREATE USER 'cdcuser'@'%' IDENTIFIED VIA mysql_native_password USING PASSWORD("cdcpwd");
-- GRANT REPLICATION SLAVE ON *.* TO 'cdcuser'@'%';
-- GRANT REPLICATION CLIENT ON *.* TO 'cdcuser'@'%';
GRANT ALL PRIVILEGES ON *.* TO 'cdcuser'@'%' ;
FLUSH PRIVILEGES;

SET GLOBAL gtid_domain_id = 1;

DROP DATABASE IF EXISTS test;
CREATE DATABASE IF NOT EXISTS test;
USE test;


CREATE TABLE `test` (
        `id` INT(11) NOT NULL,
        `name` VARCHAR(50) NOT NULL
)