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

var db_ip = "34.83.17.105:3307"
var db_url = "root:jinhyung@tcp(" + db_ip +")/"
//var db_url = "root:jinhyung@tcp(34.83.17.105:3307)/"

func SetUserInfo(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	println("----------------------------------------")
	println("[User] 진입 :: SetUserInfo")
	println("----------------------------------------")

	hugh_db_rul := db_url
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

	//해당 DB에 유저의 정보가 있는지 검색하고 있으면 Update / 없으면 insert실행

	insertContext := `user_id, user_name, user_level, user_gold`
	insertValues := `VALUES(?, ?, ?, ?)`
	insertQuery := `INSERT INTO user_data` + ` ( ` + insertContext + ` ) ` + insertValues
	_, insertErr := hugh_db.QueryContext(ctx, insertQuery,
		reqData.UserId,
		reqData.UserName,
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

	hugh_db_rul := db_url
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

	selectQuery := `SELECT * FROM user_data where user_id=?`
	selectDB, selectErr := hugh_db.QueryContext(ctx, selectQuery, reqData.UserId)
	if selectErr != nil {
		println("----------------------------------------")
		println("[User] Error :: GetUserInfo - select DB\n", selectErr.Error())
		println("----------------------------------------")
	}

	resData := UserData{}
	for selectDB.Next() {
		selectDB.Scan(
			&resData.UserID, &resData.UserName, &resData.UserGold)
	}

	resData.Message = "Success Select DB"
	resData.MessageCode = Success
	jsonData, _ := json.Marshal(resData)

	println("----------------------------------------")
	println("[User] 탈출 :: GetUserInfo")
	println("========================================")
	return string(jsonData), nil
}

func RemoveUserInfo(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	println("----------------------------------------")
	println("[User] 진입 :: RemoveUserInfo")
	println("----------------------------------------")
	
	hugh_db_rul := db_url
	hugh_db, hugh_db_err := sql.Open("mysql", hugh_db_rul+"nakama?parseTime=true")
	if hugh_db_err != nil {
		println("----------------------------------------")
		println("[User] Error :: RemoveUserInfo\n", hugh_db_rul, "\n", hugh_db_err.Error())
		println("----------------------------------------")
	}

	reqData := ReqUserInfoPacket{}
	reqErr := json.Unmarshal([]byte(payload), &reqData)
	if reqErr != nil {
		println("----------------------------------------")
		println("[User] Error :: RemoveUserInfo - request data\n", reqErr.Error())
		println("----------------------------------------")
	}

	//DB에서 해당 유저 id로 정보 삭제하기
	delteCondition := `user_id=?`
	deleteQuery := `DELETE FROM user_data WHERE ` + delteCondition
	_, deleteErr := hugh_db.QueryContext(ctx, deleteQuery, reqData.UserId)
	if deleteErr != nil {
		println("----------------------------------------")
		println("[User] Error :: GetUserInfo - select DB\n", deleteErr.Error())
		println("----------------------------------------")
	}

	resData := UserData{}
	resData.Message = "Success Remove DB"
	resData.MessageCode = Success
	jsonData, _ := json.Marshal(resData)

	println("----------------------------------------")
	println("[User] 탈출 :: RemoveUserInfo")
	println("========================================")
	return string(jsonData), nil
}
