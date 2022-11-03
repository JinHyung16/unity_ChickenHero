package main

import (
	"context"
	"database/sql"
	"fmt"

	match "HughCommon/Go/src/Match"
	user "HughCommon/Go/src/User"

	"github.com/heroiclabs/nakama-common/runtime"
)

func InitModule(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, initializer runtime.Initializer) error {
	user.RegisterUserRPC(logger, initializer)
	match.RegisterMatch(logger, initializer)
	
	fmt.Println("[Init Runtime Module] : nakama_plugin SUCCESS")
	return nil
}
