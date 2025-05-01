USE [AWCDigitalCommerce]

SELECT 'truncate table', table_name FROM information_schema.tables
WHERE table_type = 'BASE TABLE' AND table_catalog = 'AWCDigitalCommerce'
ORDER BY table_name ASC
