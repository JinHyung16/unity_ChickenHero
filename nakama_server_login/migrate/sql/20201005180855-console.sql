/*
 * Copyright 2020 The Nakama Authors
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
CREATE TABLE IF NOT EXISTS console_user (
    PRIMARY KEY (id),

    create_time  DATETIME  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    disable_time DATETIME  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    email        VARCHAR(255) NOT NULL,
    CONSTRAINT console_user_email_uniq UNIQUE (email),
    id           VARCHAR(40)         NOT NULL,
    metadata     JSON,
    password     VARBINARY(32000),
    role         SMALLINT     NOT NULL DEFAULT 4 CHECK (role >= 1), -- unused(0), admin(1), developer(2), maintainer(3), readonly(4)
    update_time  DATETIME  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    username     VARCHAR(128) NOT NULL, 
    CONSTRAINT console_user_username_uniq UNIQUE (username)
);

-- +migrate Down
ALTER TABLE user_device
    DROP COLUMN IF EXISTS preferences,
    DROP COLUMN IF EXISTS push_token_amazon,
    DROP COLUMN IF EXISTS push_token_android,
    DROP COLUMN IF EXISTS push_token_huawei,
    DROP COLUMN IF EXISTS push_token_ios,
    DROP COLUMN IF EXISTS push_token_web;

DROP TABLE IF EXISTS console_user, purchase_receipt;
