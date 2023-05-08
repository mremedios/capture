--liquibase formatted sql
--changeset indexes:7
CREATE INDEX messages_local_call_id_index ON messages USING hash(local_call_id);
CREATE UNIQUE INDEX calls_query_index ON calls USING btree(created, host, call_id);
CREATE UNIQUE INDEX available_headers_header_key ON available_headers USING btree (header);
CREATE UNIQUE INDEX headers_value_local_call_id_key ON headers USING btree (value, local_call_id);

-- it shoud be hash since pk of calls contains data
CREATE UNIQUE INDEX calls_pkey on calls USING btree(local_call_id); 

