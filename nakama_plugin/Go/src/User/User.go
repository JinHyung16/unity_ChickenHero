package User

import (
	"context"
	"database/sql"
	"encoding/json"
	"fmt"

	"HughCommon/Go/src/TableData"

	"github.com/heroiclabs/nakama-common/runtime"
)

// Usages
// GCP hugh-db VM에서 운용중인 mysql 서버 접속하는 방법
/*
hugh_db_rul := "root:mysql비번@tcp(GCP vm 외부 ip:mysql tcp port)/"
hugh_db, hugh_db_err := sql.Open("mysql", hugh_db_rul+"nakama?parseTime=true")
if hugh_db_err != nil {
	println("----------------------------------------")
	println("[User] Error :: GetUserRetainGoodsTest\n", hugh_db_rul, "\n", hugh_db_err.Error())
	println("----------------------------------------")
}
*/

func RegisterUserRPC(logger runtime.Logger, initializer runtime.Initializer) error {
	initializer.RegisterRpc("set_user_goods", SetUserRetainGoods)
	initializer.RegisterRpc("get_user_goods", GetUserRetainGoods)

	fmt.Println("[RegisterUserRPC] : SUCCESS")
	return nil
}

func SetUserRetainGoods(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	println("----------------------------------------")
	println("[User] 진입 :: SetUserRetainGoods")
	println("----------------------------------------")

	hugh_db_rul := "root:jinhyung@tcp(35.247.19.228:3307)/"
	hugh_db, hugh_db_err := sql.Open("mysql", hugh_db_rul+"nakama?parseTime=true")
	if hugh_db_err != nil {
		println("----------------------------------------")
		println("[User] Error :: GetUserRetainGoodsTest\n", hugh_db_rul, "\n", hugh_db_err.Error())
		println("----------------------------------------")
	}

	reqData := TableData.ReqSetUserPacket{}
	reqErr := json.Unmarshal([]byte(payload), &reqData)
	if reqErr != nil {
		jsonData, _ := json.Marshal(reqData)
		println("----------------------------------------")
		println("[User] Error :: SetUserRetainGoods - request data\n", reqErr.Error())
		println("----------------------------------------")
		return string(jsonData), nil
	}

	insertContext := `user_id, user_name, user_level, user_gold`
	insertValues := `VALUES(?, ?, ?, ?)`
	insertQuery := `INSERT INTO user_history` + ` ( ` + insertContext + ` ) ` + insertValues
	_, insertErr := hugh_db.QueryContext(ctx, insertQuery,
		reqData.UserId,
		reqData.UserName,
		reqData.UserLevel,
		reqData.UserGold,
	)

	if insertErr != nil {
		println("----------------------------------------")
		println("[User] Error :: SetUserRetainGoods - insert DB\n", insertErr.Error())
		println("----------------------------------------")
		jsonData, _ := json.Marshal(reqData)
		return string(jsonData), nil
	}

	println("----------------------------------------")
	println("[User] 탈출 :: SetUserRetainGoods")
	println("========================================")
	jsonData, _ := json.Marshal(reqData)
	return string(jsonData), nil
}

func GetUserRetainGoods(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	println("----------------------------------------")
	println("[User] 진입 :: GetUserRetainGoods")
	println("----------------------------------------")
	
	hugh_db_rul := "root:jinhyung@tcp(35.247.19.228:3307)/"
	hugh_db, hugh_db_err := sql.Open("mysql", hugh_db_rul+"nakama?parseTime=true")
	if hugh_db_err != nil {
		println("----------------------------------------")
		println("[User] Error :: GetUserRetainGoodsTest\n", hugh_db_rul, "\n", hugh_db_err.Error())
		println("----------------------------------------")
	}

	reqData := TableData.ReqUserInfoPacket{}
	reqErr := json.Unmarshal([]byte(payload), &reqData)
	if reqErr != nil {
		println("----------------------------------------")
		println("[User] Error :: SetUserRetainGoods - request data\n", reqErr.Error())
		println("----------------------------------------")
	}

	selectQuery := `SELECT * FROM user_history where user_id=?`
	selectDB, selectErr := hugh_db.QueryContext(ctx, selectQuery, reqData.UserId)
	if selectErr != nil {
		println("----------------------------------------")
		println("[User] Error :: SetUserRetainGoods - select DB\n", selectErr.Error())
		println("----------------------------------------")
	}

	resData := TableData.UserData{}
	for selectDB.Next() {
		selectDB.Scan(
			&resData.UserID, &resData.UserName,
			&resData.UserLevel, &resData.UserGold)
	}

	resData.Message = "Success Select DB"
	resData.MessageCode = TableData.Success
	jsonData, _ := json.Marshal(resData)

	println("----------------------------------------")
	println("[User] 탈출 :: GetUserRetainGoods")
	println("========================================")
	return string(jsonData), nil
}
