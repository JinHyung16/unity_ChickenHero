/*
 * Copyright 2018 The Nakama Authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

-- +migrate Up 
CREATE TABLE IF NOT EXISTS users (
    PRIMARY KEY (id),

    id            VARCHAR(40)   NOT NULL,
    username      VARCHAR(128)  NOT NULL,
    CONSTRAINT users_username_key UNIQUE (username),
    display_name  VARCHAR(255),
    avatar_url    VARCHAR(512),
    lang_tag      VARCHAR(18)   NOT NULL DEFAULT 'en',
    location      VARCHAR(255),
    timezone      VARCHAR(255),
    metadata      JSON,
    wallet        JSON,
    email         VARCHAR(255)  UNIQUE,
    password      VARBINARY(32000),
    facebook_id   VARCHAR(128)  UNIQUE,
    google_id     VARCHAR(128)  UNIQUE,
    gamecenter_id VARCHAR(128)  UNIQUE,
    steam_id      VARCHAR(128)  UNIQUE,
    custom_id     VARCHAR(128)  UNIQUE,
    edge_count    INT        NOT NULL DEFAULT 0 CHECK (edge_count >= 0),
    create_time   DATETIME   NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_time   DATETIME   NOT NULL DEFAULT CURRENT_TIMESTAMP,
    verify_time   DATETIME   NOT NULL DEFAULT '1970-01-01 00:00:00',
    disable_time  DATETIME   NOT NULL DEFAULT '1970-01-01 00:00:00'
);

-- Setup System user.
INSERT INTO users (id, username) 
    VALUES ('00000000-0000-0000-0000-000000000000', '') 
    ON DUPLICATE KEY UPDATE id=id;  

CREATE TABLE IF NOT EXISTS user_device (
    PRIMARY KEY (id),

    id      VARCHAR(128) NOT NULL,
    user_id VARCHAR(40) NOT NULL,
    UNIQUE (user_id, id),
    preferences        JSON,
    push_token_amazon  VARCHAR(512) NOT NULL DEFAULT '',
    push_token_android VARCHAR(512) NOT NULL DEFAULT '',
    push_token_huawei  VARCHAR(512) NOT NULL DEFAULT '',
    push_token_ios     VARCHAR(512) NOT NULL DEFAULT '',
    push_token_web     VARCHAR(512) NOT NULL DEFAULT ''
);


CREATE TABLE IF NOT EXISTS notification (
    PRIMARY KEY (user_id, create_time, id),    

    id          VARCHAR(40) NOT NULL,
    CONSTRAINT notification_id_key UNIQUE (id),
    user_id     VARCHAR(40) NOT NULL,
    subject     VARCHAR(255) NOT NULL,
    content     JSON,
    code        SMALLINT NOT NULL, -- Negative values are system reserved.
    sender_id   VARCHAR(40) NOT NULL,
    create_time DATETIME   NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS storage (
    PRIMARY KEY (collection, read_val, key_val, user_id),

    collection  VARCHAR(128) NOT NULL,
    key_val     VARCHAR(128) NOT NULL,
    user_id     VARCHAR(40)  NOT NULL,
    value_val   JSON,
    version     VARCHAR(32)  NOT NULL, -- md5 hash of value object.
    read_val    SMALLINT     NOT NULL DEFAULT 1,
    write_val   SMALLINT     NOT NULL DEFAULT 1,
    create_time DATETIME  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_time DATETIME  NOT NULL DEFAULT CURRENT_TIMESTAMP,

    INDEX collection_read_user_id_key_idx (collection, read_val, user_id, key_val),
    INDEX collection_user_id_read_key_idx (collection, user_id, read_val, key_val),
    INDEX storage_auto_index_fk_user_id_ref_users (user_id),
    UNIQUE (collection, key_val, user_id)
);

-- +migrate Down 
DROP TABLE IF EXISTS 
    group_edge, groups, user_tombstone, wallet_ledger, leaderboard_record, leaderboard, message, storage, notification, user_edge, user_device, users;