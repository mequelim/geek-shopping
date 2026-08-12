-- Active: 1777159470116@@localhost@5432@geek_shopping_discount_coupon_api_db

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

-- Listing all columns within the table `coupons` and their respective data types:
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
    table_name = 'coupons'
ORDER BY ordinal_position;

-- Creating coupons:
INSERT INTO coupons(coupon_id, coupon_code, coupon_discount_amount)
VALUES
    ('80180dbb-31ea-4bf8-9237-0bf67cf24d1b', 'COUPON_10', 10),
    ('8931feef-d848-4540-8e8f-9ace1008ad52', 'COUPON_15', 15);

-- Listing data from the table `coupons`:
SELECT
    cp.coupon_id,
    cp.coupon_code,
    cp.coupon_discount_amount
FROM coupons cp
ORDER BY cp.coupon_id;