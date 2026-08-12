-- Active: 1777159470116@@localhost@5432@geek_shopping_identity_server_db

-- Listing all tables within the database:
SELECT
    tab.table_schema,
    tab.table_name,
    tab.table_type
FROM information_schema.tables tab
WHERE
    table_type = 'BASE TABLE' AND
    table_schema NOT IN ('pg_catalog', 'information_schema')
ORDER BY
    table_schema,
    table_name;

-- Listing all columns within the table `asp_net_users` and their respective data types:
SELECT
    col.table_name,
    col.column_name,
    col.data_type,
    col.character_maximum_length,
    col.is_nullable,
    col.ordinal_position,
    col.maximum_cardinality,
    col.column_default,
    col.domain_name,
    col.domain_schema
FROM information_schema.columns col
WHERE
    table_schema = 'public' AND
    table_name = 'AspNetUsers'
ORDER BY ordinal_position;

-- Listing data from the `AspNetUsers` table:
SELECT *
FROM "AspNetUsers" users
ORDER BY users."Id";

-- Listing data from the `AspNetRoles` table:
SELECT *
FROM "AspNetRoles" roles
ORDER BY roles."Name";