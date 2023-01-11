--liquibase formatted sql
--changeset test:2
create table sources (
    raw_id bigserial not null,
    created_date timestamp with time zone default current_timestamp not null,
    protocol_header jsonb NOT NULL,
    raw varchar not null,
    unique(raw_id)
)