package User

import (
	"context"
	"encoding/json"
	"database/sql"

	"HughCommon/Go/src/TableData"

	"github.com/heroiclabs/nakama-common/runtime"
)

func RegisterUserRPC(logger runtime.Logger, initializer runtime.Initializer) error {
	initializer.RegisterRpc("set_user_goods", SetUserRetainGoods)
	initializer.RegisterRpc("get_user_goods", GetUserRetainGoods)
	initializer.RegisterRpc("get_user_goods_test", GetUserRetainGoodsTest)
	
	return nil
}

func SetUserRetainGoods(ctx context.Context, logger runtime.Logger, db *runtime.DBManager, nk runtime.NakamaModule, payload string) (string, error) {
	println("----------------------------------------")
	println("[User] 진입 :: SetUserRetainGoods")
	println("----------------------------------------")
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
	insertQuery := `INSERT INTO user` + ` ( ` + insertContext + ` ) ` + insertValues
	_, insertErr := db.Hugh_db.QueryContext(ctx, insertQuery,
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

func GetUserRetainGoods(ctx context.Context, logger runtime.Logger, db *runtime.DBManager, nk runtime.NakamaModule, payload string) (string, error) {
	println("----------------------------------------")
	println("[User] 진입 :: GetUserRetainGoods")
	println("----------------------------------------")
	reqData := TableData.ReqUserInfoPacket{}
	reqErr := json.Unmarshal([]byte(payload), &reqData)
	if reqErr != nil {
		println("----------------------------------------")
		println("[User] Error :: SetUserRetainGoods - request data\n", reqErr.Error())
		println("----------------------------------------")
	}

	selectQuery := `SELECT * FROM user where user_id=?`
	selectDB, selectErr := db.Hugh_db.QueryContext(ctx, selectQuery, reqData.UserId)
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

func GetUserRetainGoodsTest(ctx context.Context, logger runtime.Logger, db *runtime.DBManager, nk runtime.NakamaModule, payload string) (string, error) {
	println("----------------------------------------")
	println("[User] 진입 :: GetUserRetainGoodsTest")
	println("----------------------------------------")

	hugh_db_rul := "root:jinhyung@tcp(34.82.70.174:3306)/"
	hugh_db, hugh_db_err := sql.Open("mysql", hugh_db_rul+"nakama?parseTime=true")
	if hugh_db_err != nil {
		println("----------------------------------------")
		println("[User] Error :: GetUserRetainGoodsTest\n", hugh_db_rul, "\n", hugh_db_err.Error())
		println("----------------------------------------")
	}

	selectQuery := `SELECT * FROM user`

	selectDB, selectErr := hugh_db.QueryContext(ctx, selectQuery)
	if selectErr != nil {
		println("----------------------------------------")
		println("[User] Error :: GetUserRetainGoodsTest - select DB\n", selectErr.Error())
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
	println("[User] 탈출 :: GetUserRetainGoodsTest")
	println("========================================")
	return string(jsonData), nil
}
