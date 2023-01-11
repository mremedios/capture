--liquibase formatted sql
--changeset test:1
create table sources
(
    created_date    timestamp with time zone default current_timestamp not null,
    protocol_header jsonb                                              not null,
    raw             varchar                                            not null
);

-- later...
-- create index datagin on sources using gin (protocol_header);

