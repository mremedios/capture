--liquibase formatted sql
--changeset available_headers:1
CREATE TABLE available_headers
(
    header VARCHAR(100) NOT NULL,
    UNIQUE (header)
);

-- --changeset insert:2
-- INSERT INTO available_headers (header)
-- VALUES ('callid'),
--        ('callsessionid'),
--        ('tcommuniactionid') 