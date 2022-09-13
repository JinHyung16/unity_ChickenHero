package main

import (
	"context"
	"fmt"

	//app "HughCommon/Go/src/App"
	user "HughCommon/Go/src/User"

	"github.com/heroiclabs/nakama-common/runtime"
)

func InitModule(ctx context.Context, logger runtime.Logger, db *runtime.DBManager, nk runtime.NakamaModule, initializer runtime.Initializer) error {

	user.RegisterUserRPC(logger, initializer)


	fmt.Println("[Init Runtime Module] : 0518_1800")
	return nil
}
