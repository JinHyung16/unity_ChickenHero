package main

import (
	"context"
	"fmt"
	"database/sql"

	"github.com/heroiclabs/nakama-common/runtime"
)

func InitModule(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, initializer runtime.Initializer) error {

	// user.go rpc 등록
	initializer.RegisterRpc("set_user_goods", SetUserRetainGoods)
	initializer.RegisterRpc("get_user_goods", GetUserRetainGoods)

	fmt.Println("[Init Runtime Module] : nakama_plugin SUCCESS")
	return nil
}