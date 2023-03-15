--liquibase formatted sql

--changeset create type
CREATE TYPE header_type AS
(
    header        varchar(50),
    value         varchar(256),
    local_call_id bigint
);

--changeset insert
CREATE OR replace PROCEDURE insert_headers(arr header_type[])
    LANGUAGE SQL AS
$$
INSERT INTO headers(header, value, local_call_id)
SELECT *
FROM unnest(arr)
ON CONFLICT DO NOTHING;
$$;
