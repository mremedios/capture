--liquibase formatted sql
--changeset messages
CREATE TABLE messages
(
    message_id      BIGSERIAL NOT NULL,
    protocol_header TEXT NOT NULL,
    message         TEXT NOT NULL,
    local_call_id   BIGSERIAL NOT NULL,
    FOREIGN KEY (local_call_id) REFERENCES calls (local_call_id),
    PRIMARY KEY (message_id)
); 