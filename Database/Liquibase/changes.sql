--liquibase formatted sql

--changeset message:9
ALTER TABLE messages 
ADD message_id bigserial;