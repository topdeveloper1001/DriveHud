CREATE OR REPLACE FUNCTION truncate_tables(_username text)
  RETURNS void AS
$func$
DECLARE
   _tbl text;
   _sch text;
BEGIN
   FOR _sch, _tbl IN 
      SELECT schemaname, tablename
      FROM   pg_tables
      WHERE  tableowner = _username AND tablename <> 'VersionInfo'
      AND    schemaname = 'public'
   LOOP
      -- RAISE NOTICE '%',
       EXECUTE  -- dangerous, test before you execute!
      'TRUNCATE TABLE ' || quote_ident(_sch) || '.' || quote_ident(_tbl)  || ' RESTART IDENTITY' || ' CASCADE';
   END LOOP;
END
$func$ LANGUAGE plpgsql;

select truncate_tables('postgres');