--liquibase formatted sql

--changeset message:9
ALTER TABLE messages 
ADD message_id bigserial;  
    
--changeset methods:10
CREATE TABLE methods
(
    method VARCHAR(100) NOT NULL,
    UNIQUE (method)
);