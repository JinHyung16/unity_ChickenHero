package main

import (
	"context"
	"database/sql"
	"fmt"

	"github.com/heroiclabs/nakama-common/runtime"
)

func InitModule(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, initializer runtime.Initializer) error {

	// user.go rpc 등록
	initializer.RegisterRpc("set_user_info", SetUserInfo)
	initializer.RegisterRpc("get_user_info", GetUserInfo)
	initializer.RegisterRpc("remove_user_info", RemoveUserInfo)

	fmt.Println("[Init Runtime Module] : nakama_plugin SUCCESS")
	return nil
}
