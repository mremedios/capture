--liquibase formatted sql
--changeset test:3
create table headers(
    header_id smallint not null,
    value varchar not null,
    created_date timestamp with time zone default current_timestamp not null,
    raw_id bigint not null,
    constraint fk_header_id foreign key (header_id) references available_headers(header_id),
    constraint fk_source_id foreign key (raw_id) references sources(raw_id)
)