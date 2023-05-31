--liquibase formatted sql

--changeset indexes:7
-- CREATE UNIQUE INDEX available_headers_header_key ON available_headers USING btree (header); --создается автоматически
-- CREATE UNIQUE INDEX available_headers_header_key ON methods USING btree (method);
CREATE INDEX messages_local_call_id_index ON messages USING hash (local_call_id); -- joining with headers
CREATE INDEX headers_value_idx ON headers USING hash (value); -- select by value
CREATE UNIQUE INDEX headers_pkey ON headers USING btree (value, local_call_id, at); -- unique constraint
CREATE INDEX calls_search_query on calls using btree (host, call_id); -- to search call

-- CREATE INDEX calls_call_id on calls USING btree (local_call_id); 
-- CREATE UNIQUE INDEX calls_query_index ON calls USING btree (at, host);
-- CREATE INDEX headers_value_local_call_id_key ON headers USING btree (value, local_call_id);

