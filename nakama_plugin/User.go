package main

import (
	"context"
	"database/sql"
	"encoding/json"

	"github.com/heroiclabs/nakama-common/runtime"
)

// Usages
// GCP hugh-db VM에서 운용중인 mysql 서버 접속하는 방법
// GCP 재시작시 항상 외부 IP바뀌니깐, 여기서도 바꿔줘야 한다.

/*
hugh_db_url := "root:mysql비번@tcp(GCP vm 외부 ip:mysql tcp port)/"
hugh_db, hugh_db_err := sql.Open("mysql", hugh_db_rul+"nakama?parseTime=true")
if hugh_db_err != nil {
	println("----------------------------------------")
	println("[User] Error :: GetUserRetainGoodsTest\n", hugh_db_rul, "\n", hugh_db_err.Error())
	println("----------------------------------------")
}
*/

func SetUserInfo(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	println("----------------------------------------")
	println("[User] 진입 :: SetUserInfo")
	println("----------------------------------------")

	hugh_db_rul := "root:jinhyung@tcp(34.83.17.105:3307)/"
	hugh_db, hugh_db_err := sql.Open("mysql", hugh_db_rul+"nakama?parseTime=true")
	if hugh_db_err != nil {
		println("----------------------------------------")
		println("[User] Error :: SetUserInfo\n", hugh_db_rul, "\n", hugh_db_err.Error())
		println("----------------------------------------")
	}

	reqData := ReqSetUserPacket{}
	reqErr := json.Unmarshal([]byte(payload), &reqData)
	if reqErr != nil {
		jsonData, _ := json.Marshal(reqData)
		println("----------------------------------------")
		println("[User] Error :: SetUserInfo - request data\n", reqErr.Error())
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
		println("[User] Error :: SetUserInfo - insert DB\n", insertErr.Error())
		println("----------------------------------------")
		jsonData, _ := json.Marshal(reqData)
		return string(jsonData), nil
	}

	println("----------------------------------------")
	println("[User] 탈출 :: SetUserInfo")
	println("========================================")
	jsonData, _ := json.Marshal(reqData)
	return string(jsonData), nil
}

func GetUserInfo(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	println("----------------------------------------")
	println("[User] 진입 :: GetUserInfo")
	println("----------------------------------------")

	hugh_db_rul := "root:jinhyung@tcp(34.83.17.105:3307)/"
	hugh_db, hugh_db_err := sql.Open("mysql", hugh_db_rul+"nakama?parseTime=true")
	if hugh_db_err != nil {
		println("----------------------------------------")
		println("[User] Error :: GetUserInfo\n", hugh_db_rul, "\n", hugh_db_err.Error())
		println("----------------------------------------")
	}

	reqData := ReqUserInfoPacket{}
	reqErr := json.Unmarshal([]byte(payload), &reqData)
	if reqErr != nil {
		println("----------------------------------------")
		println("[User] Error :: GetUserInfo - request data\n", reqErr.Error())
		println("----------------------------------------")
	}

	selectQuery := `SELECT * FROM user_history where user_id=?`
	selectDB, selectErr := hugh_db.QueryContext(ctx, selectQuery, reqData.UserId)
	if selectErr != nil {
		println("----------------------------------------")
		println("[User] Error :: GetUserInfo - select DB\n", selectErr.Error())
		println("----------------------------------------")
	}

	resData := UserData{}
	for selectDB.Next() {
		selectDB.Scan(
			&resData.UserID, &resData.UserName,
			&resData.UserLevel, &resData.UserGold)
	}

	resData.Message = "Success Select DB"
	resData.MessageCode = Success
	jsonData, _ := json.Marshal(resData)

	println("----------------------------------------")
	println("[User] 탈출 :: GetUserInfo")
	println("========================================")
	return string(jsonData), nil
}
