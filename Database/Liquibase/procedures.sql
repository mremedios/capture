--liquibase formatted sql

--changeset type:5
CREATE TYPE header_type AS
(
    header        VARCHAR(100),
    at            DATE,
    value         TEXT,
    local_call_id INT
);

--changeset insert_headers:6
CREATE OR REPLACE PROCEDURE insert_headers(arr header_type[])
    LANGUAGE SQL AS
$$
INSERT INTO partman.headers(header, at, value, local_call_id)
SELECT *
FROM unnest(arr)
ON CONFLICT DO NOTHING
$$;
