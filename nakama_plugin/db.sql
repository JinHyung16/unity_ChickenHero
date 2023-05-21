create table user_data (
    user_id VARCHAR(255) NOT NULL primary key,
    user_name VARCHAR(255) NOT NULL,
    user_gold INT NULL DEFAULT 5000,
)