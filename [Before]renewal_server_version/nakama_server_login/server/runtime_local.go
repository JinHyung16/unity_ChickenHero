package server

import (
	"context"
	"database/sql"
	"encoding/json"
	"fmt"

	"github.com/heroiclabs/nakama-common/runtime"
	"github.com/heroiclabs/nakama/v3/utils"
)

type ChangeState struct {
	Useruid string `json:"useruid"`
	// 0 정상 1 제재
	State int `json:"state"`
}

type ResUserState struct {
	Messagecode int    `json:"messagecode"`
	Useruid     string `json:"useruid"`
	State       int    `json:"state"`
}

type ResUserList struct {
	Messagecode int            `json:"messagecode"`
	UserList    []DB_UserState `json:"userlist"`
}

type DB_UserState struct {
	Useruid string `json:"userid"`
	// 0 정상 1 제재
	State int `json:"userstate"`
}

func LocalUserState(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	fmt.Println("[runtime_local.go] LocalUserState : 진입")
	var changeData ChangeState
	json.Unmarshal([]byte(payload), &changeData)

	var result ResUserState
	result.Useruid = changeData.Useruid
	result.State = changeData.State

	query := `SELECT COUNT(*) FROM nakama.userstate WHERE userid = ?`
	var count int
	res := db.QueryRow(query, changeData.Useruid)
	res.Scan(&count)

	if count > 0 {
		updatequery := `UPDATE nakama.userstate SET currentstate = ? WHERE userid = ?;`
		logquery := `INSERT INTO nakama.userstate_log (userid, changestate) VALUES (?, ?);`

		_, err := db.Exec(updatequery, changeData.State, result.Useruid)
		_, err2 := db.Exec(logquery, result.Useruid, changeData.State)

		utils.HandleError(err)
		utils.HandleError(err2)
	} else {

		insertquery := `INSERT INTO nakama.userstate (userid, currentstate) VALUES (?, 0);`
		logquery := `INSERT INTO nakama.userstate_log (userid, changestate) VALUES (?, 0);`

		_, err := db.Exec(insertquery, changeData.Useruid)
		_, err2 := db.Exec(logquery, changeData.Useruid)

		utils.HandleError(err)
		utils.HandleError(err2)

		result.Messagecode = 0
	}

	resjson, _ := json.Marshal(result)
	fmt.Println("[runtime_local.go] LocalUserState : 탈출")
	return string(resjson), nil
}

func ListUserStatus(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	fmt.Println("[runtime_local.go] ListUserStatus : 진입")
	var userstate []DB_UserState

	getquery := `SELECT userid, currentstate FROM nakama.userstate WHERE currentstate = 1`
	resscan, err := db.QueryContext(ctx, getquery)
	defer resscan.Close()

	if err != nil {
		utils.HandleError(err)
		resjson, _ := json.Marshal(userstate)
		return string(resjson), err
	}

	for resscan.Next() {
		var userid string
		var currentstate int

		err := resscan.Scan(&userid, &currentstate)
		utils.HandleError(err)

		data := DB_UserState{
			Useruid: userid,
			State:   currentstate,
		}

		userstate = append(userstate, data)
	}

	resjson, _ := json.Marshal(userstate)
	fmt.Println("[runtime_local.go] ListUserStatus : 탈출")
	return string(resjson), nil
}
