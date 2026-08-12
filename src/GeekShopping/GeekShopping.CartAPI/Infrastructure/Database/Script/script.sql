-- Active: 1777159470116@@localhost@5432@geek_shopping_cart_api_db

-- Listing all existing tables within the database:
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

-- Listing all columns within the table `cart_details` and their respective data types:
SELECT
    col.table_name,
    col.column_name,
    col.data_type,
    col.numeric_precision,
    col.numeric_scale,
    col.character_maximum_length,
    col.datetime_precision,
    col.is_nullable,
    col.is_updatable,
    col.ordinal_position,
    col.maximum_cardinality,
    col.column_default,
    col.domain_name,
    col.domain_schema
FROM information_schema.columns col
WHERE
    table_schema = 'public' AND
    table_name = 'cart_details'
ORDER BY ordinal_position;

-- Listing all columns within the table `cart_header` and their respective data types:
SELECT *
FROM information_schema.columns col
WHERE
    table_schema = 'public' AND
    table_name = 'cart_header'
ORDER BY ordinal_position;

-- Listing all columns within the table `products` and their respective data types:
SELECT *
FROM information_schema.columns col
WHERE
    table_schema = 'public' AND
    table_name = 'products'
ORDER BY ordinal_position;

-- Listing all the products in the cart:
SELECT *
FROM cart_details cd
ORDER BY cd.count;

-- Listing all the products in the cart header:
SELECT *
FROM cart_header ch
ORDER BY
    ch.user_id,
    ch.cart_id;