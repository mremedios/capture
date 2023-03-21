--liquibase formatted sql
--changeset available_headers
CREATE TABLE available_headers
(
    header VARCHAR(100) NOT NULL,
    UNIQUE (header)
);

--changeset insert sample values in available_headers
INSERT INTO available_headers (header)
VALUES ('callid'),
       ('callsessionid'),
       ('tcommuniactionid') 