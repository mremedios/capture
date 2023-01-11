--liquibase formatted sql
--changeset test:1
create table available_headers (
    header_id smallserial not null,
    header varchar not null,
    unique(header_id)
)