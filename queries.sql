------------------------------- CALLS ---------------------------------------------------------------------------------------------

INSERT INTO calls (call_id, created, host)
VALUES (@p0, @p1, @p2)
RETURNING local_call_id;

SELECT c.local_call_id, c.call_id, c.created, c.host
FROM calls AS c
WHERE c.call_id = @__data_CallId_0 AND c.host = @__ToString_1 AND c.created > (now() + INTERVAL '-2 hours')
LIMIT 1

------------------------------- MESSAGES --------------------------------------------------------------

INSERT INTO messages (details, local_call_id, message)
VALUES (@p1119, @p1120, @p1121)
RETURNING message_id;

------------------------------- HEADERS ---------------------------------------------------------------------------------------------

INSERT INTO headers(header, value, local_call_id)
SELECT *
FROM unnest(arr)
ON CONFLICT DO NOTHING

SELECT m.message_id, m.details, m.local_call_id, m.message
FROM headers AS h
         INNER JOIN messages AS m ON h.local_call_id = m.local_call_id
WHERE h.value = @__header_0
