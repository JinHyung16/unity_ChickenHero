create table user (
    user_index int NOT NULL AUTO_INCREMENT PRIMARY KEY,
    user_id varchar(40) NOT NULL,
    user_name varchar(24) NOT NULL,
    user_level int NOT NULL DEFAULT 0,
    user_gold int NOT NULL DEFAULT 0,
)