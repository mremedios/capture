--liquibase formatted sql

--changeset partman:8

SELECT create_parent('partman.calls', 'at', 'native', 'daily', p_premake := 1);
SELECT create_parent('partman.messages', 'at', 'native', 'daily', p_premake := 1);
SELECT create_parent('partman.headers', 'at', 'native', 'daily', p_premake := 1);

UPDATE part_config
SET retention = '2 days', premake = 1, retention_keep_table=false;

SET constraint_exclusion = on;

CREATE OR REPLACE PROCEDURE set_retention(days varchar(16))
    LANGUAGE SQL AS
$$
UPDATE part_config
SET retention = days
$$;

select run_maintenance();

