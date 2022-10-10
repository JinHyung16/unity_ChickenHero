package server

import (
	"context"
	"database/sql"
	"log"
	"fmt"

	"github.com/heroiclabs/nakama/v3/utils"
)

func CheckUserState(ctx context.Context, db *sql.DB, uid string) bool {
	fmt.Println("[api_local.go] CheckUserState : 진입")
	countquery := `SELECT COUNT(*) FROM nakama.userstate WHERE userid = ?`
	var count int
	res := db.QueryRow(countquery, uid)
	res.Scan(&count)

	log.Printf("유저 상태 로그인 체크 : %s\n", countquery)

	if count == 0 {
		insertquery := `INSERT INTO nakama.userstate (userid, currentstate) VALUES (?, 0);`
		logquery := `INSERT INTO nakama.userstate_log (userid, changestate) VALUES (?, 0);`

		_, err := db.Exec(insertquery, uid)
		_, err2 := db.Exec(logquery, uid)

		utils.HandleError(err)
		utils.HandleError(err2)
		return true
	} else {
		findQuery := `SELECT currentstate FROM nakama.userstate WHERE userid = ?`
		var currentstate int
		res := db.QueryRow(findQuery, uid)
		err := res.Scan(&currentstate)
		utils.HandleError(err)

		if currentstate != 0 {
			return false
		}
	}

	fmt.Println("[api_local.go] CheckUserState : 탈출")
	return true
}
