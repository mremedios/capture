--liquibase formatted sql

--changeset type:5
CREATE TYPE header_type AS
(
    header        varchar(50),
    value         varchar(256),
    local_call_id bigint
);

--changeset insert_headers:6
CREATE OR REPLACE PROCEDURE insert_headers(arr header_type[])
    LANGUAGE SQL AS
$$
INSERT INTO headers(header, value, local_call_id)
SELECT *
FROM unnest(arr)
ON CONFLICT DO NOTHING
$$;
