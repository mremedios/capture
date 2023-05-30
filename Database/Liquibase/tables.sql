--liquibase formatted sql

--changeset available_headers:1
CREATE TABLE available_headers
(
    header VARCHAR(100) NOT NULL,
    UNIQUE (header)
);

--changeset calls:2
CREATE TABLE calls
(
    local_call_id BIGSERIAL                                          NOT NULL,
    at            TIMESTAMP WITH time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    host          VARCHAR(100)                                       NOT NULL,
    call_id       VARCHAR(100)                                       NOT NULL,
    PRIMARY KEY (local_call_id, at)
) PARTITION BY RANGE (at);
    
--changeset messages:3
CREATE TABLE messages
(
    at            DATE DEFAULT CURRENT_DATE NOT NULL,
    message       TEXT                      NOT NULL,
    local_call_id INT                       NOT NULL,
    details       TEXT                      NOT NULL
) PARTITION BY RANGE (at);

--changeset headers:4
CREATE TABLE headers
(
    header        VARCHAR(100)              NOT NULL,
    at            DATE DEFAULT CURRENT_DATE NOT NULL,
    value         TEXT                      NOT NULL,
    local_call_id BIGINT                    NOT NULL,
    UNIQUE (value, local_call_id, at)
) PARTITION BY RANGE (at);