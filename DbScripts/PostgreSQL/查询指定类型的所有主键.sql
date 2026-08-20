SELECT
    n.nspname AS schema_name,               -- 所在schema（模式/命名空间）
    c.relname AS table_name,                -- 表名
    a.attname AS primary_key_column,        -- 主键列名
    format_type(a.atttypid, a.atttypmod) AS data_type -- 数据类型，如int4
FROM
    pg_class c                              -- 存储所有表和索引
    JOIN pg_namespace n ON n.oid = c.relnamespace  -- 获取schema信息
    JOIN pg_index i ON i.indrelid = c.oid   -- 连接索引信息
    JOIN pg_attribute a ON a.attrelid = c.oid
                         AND a.attnum = ANY(i.indkey) -- 关联主键列
    JOIN pg_type t ON t.oid = a.atttypid    -- 关联数据类型
WHERE
    c.relkind = 'r'                         -- 'r' 代表普通表 (regular table)
    AND i.indisprimary = True               -- 只取主键索引
    AND t.typname = 'int4'                  -- 主键列的类型名为 'int4'
    AND n.nspname NOT IN ('pg_catalog', 'information_schema'); -- 排除系统表