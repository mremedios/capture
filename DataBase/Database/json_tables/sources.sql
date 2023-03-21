--liquibase formatted sql
--changeset test:1
drop table if exists sources;
create table sources
(
    created_date    timestamp with time zone default current_timestamp not null,
    endpoint        varchar(20)                                        not null,
    call_id         varchar(100)                                       not null,
    protocol_header jsonb                                              not null,
    raw             varchar                                            not null
);

-- later...
-- create index datagin on sources using gin (protocol_header);

--changeset test:2
drop table if exists available_headers;
create table available_headers
(
    header varchar not null,
    unique (header)
);

--changeset test:insert sample values
insert into available_headers(header)
values ('callid'),
       ('callsessionid'),
       ('tcommuniactionid')