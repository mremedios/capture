--liquibase formatted sql
--changeset headers:2
CREATE TABLE headers
(
    header        VARCHAR(100) NOT NULL,
    value         VARCHAR(256) NOT NULL,
    local_call_id BIGINT       NOT NULL,
    CONSTRAINT fk_source_id FOREIGN KEY (local_call_id) REFERENCES calls (local_call_id),
    UNIQUE (value, local_call_id)
) 