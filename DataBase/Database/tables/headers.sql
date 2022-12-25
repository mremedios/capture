

create table headers(
    header_id smallint not null,
    value varchar not null,
    created_date timestamp with time zone default current_timestamp not null,
    raw_id bigint not null
)