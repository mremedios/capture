--liquibase formatted sql
--changeset calls
CREATE TABLE calls
(
    local_call_id BIGSERIAL                                          NOT NULL,
    created       TIMESTAMP WITH time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    host          VARCHAR(20)                                        NOT NULL,
    call_id       VARCHAR(100)                                       NOT NULL,
    PRIMARY KEY (local_call_id)
); 

