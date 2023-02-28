--liquibase formatted sql

create type headertype as
(header varchar(50),
 value varchar(256),
 local_call_id bigint);


--changeset insert
CREATE or replace PROCEDURE insert_data(aaa headertype[])
    LANGUAGE SQL
AS $$
INSERT INTO headers(header, value, local_call_id)
select * from unnest(aaa)
on conflict do nothing;
$$;