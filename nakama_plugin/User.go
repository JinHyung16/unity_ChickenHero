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

var db_ip = "34.168.1.26" //hugh_db VIM의 외부 IP
var db_url = "root:jinhyung@tcp(" + db_ip + ":3307)/"

// var db_url = "root:jinhyung@tcp(34.83.17.105:3307)/"

func LogIn(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	println("----------------------------------------")
	println("[User] 진입 :: LogIn")
	println("----------------------------------------")

	hugh_db_rul := db_url
	hugh_db, hugh_db_err := sql.Open("mysql", hugh_db_rul+"nakama?parseTime=true")
	if hugh_db_err != nil {
		println("----------------------------------------")
		println("[User] Error :: LogIn\n", hugh_db_rul, "\n", hugh_db_err.Error())
		println("----------------------------------------")
	}

	reqData := ReqSetUserPacket{}
	reqErr := json.Unmarshal([]byte(payload), &reqData)
	if reqErr != nil {
		jsonData, _ := json.Marshal(reqData)
		println("----------------------------------------")
		println("[User] Error :: LogIn - request data\n", reqErr.Error())
		println("----------------------------------------")
		return string(jsonData), nil
	}

	//	insertQuery := `INSERT INTO user_data user_id (user_id, user_name) VALUES (?, ?)`
	//	_, insertErr := hugh_db.QueryContext(ctx, insertQuery, reqData.UserId, reqData.UserName)
	//	if insertErr != nil {
	//		println("----------------------------------------")
	//		println("[User] Error :: SetUserInfo - insert data\n", insertErr.Error())
	//		println("----------------------------------------")
	//	}

	userOldName, _ := ctx.Value(runtime.RUNTIME_CTX_USERNAME).(string) // 해당 닉네임 값은 올바르지 않은 경우가 있다.

	selectQuery := `SELECT user_name FROM user_data where user_id=?`
	selectDB, selectErr := hugh_db.QueryContext(ctx, selectQuery, reqData.UserId)
	if selectErr != nil {
		println("----------------------------------------")
		println("[User] Error :: LogIn - select DB\n", selectErr.Error())
		println("----------------------------------------")
	}
	selectDB.Scan(&userOldName)
	println("----------------------------------------")
	println("[User] DP에서 가져온 user name :\n", selectDB)
	println("----------------------------------------")

	insertQuery := `INSERT INTO user_data (user_id, user_name) VALUES(?, ?) ON DUPLICATE KEY UPDATE
	user_name=?`
	_, insertErr := hugh_db.QueryContext(ctx, insertQuery, reqData.UserId, userOldName, reqData.UserName)
	if insertErr != nil {
		println("----------------------------------------")
		println("[User] Error :: LogIn - insert data\n", insertErr.Error())
		println("----------------------------------------")
	}

	println("----------------------------------------")
	println("[User] 탈출 :: LogIn")
	println("========================================")
	jsonData, _ := json.Marshal(reqData)
	return string(jsonData), nil
}

func GetUser(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
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
			&resData.UserID, &resData.UserName)
	}

	resData.Message = "Success Select DB"
	resData.MessageCode = Success
	jsonData, _ := json.Marshal(resData)

	println("----------------------------------------")
	println("[User] 탈출 :: GetUserInfo")
	println("========================================")
	return string(jsonData), nil
}

func RemoveUser(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
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
