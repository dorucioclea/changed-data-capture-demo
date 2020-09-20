
RESET MASTER;
CREATE USER 'cdcuser'@'%' IDENTIFIED BY 'cdcpwd';
GRANT REPLICATION SLAVE ON *.* TO 'cdcuser'@'%';
GRANT REPLICATION CLIENT ON *.* TO 'cdcuser'@'%';
FLUSH PRIVILEGES;

DROP DATABASE IF EXISTS test;
CREATE DATABASE IF NOT EXISTS test;
USE test;

CREATE TABLE Author
( 
        Id INT(11) NOT NULL AUTO_INCREMENT,
        FirstName VARCHAR(45) NOT NULL,
        LastName VARCHAR(45) NOT NULL,
        NickName VARCHAR(45) NOT NULL,
        CONSTRAINT authors_PK PRIMARY KEY (Id)
);

CREATE TABLE Book
(
        Id INT(11) NOT NULL AUTO_INCREMENT,

        BookName VARCHAR(45) NOT NULL,

        AuthorId int(11) NOT NULL,

        CONSTRAINT book_authors
                FOREIGN KEY (AuthorId)
                REFERENCES Author (Id) 

        CONSTRAINT books_pk 
                PRIMARY KEY (Id)
)
