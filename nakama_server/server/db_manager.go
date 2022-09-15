package server

import (
	//"context"
	"database/sql"
	"fmt"

	//_ "github.com/go-sql-driver/mysql"
	"github.com/heroiclabs/nakama-common/runtime"
)

var (
	DBManager       *runtime.DBManager
	Game_Channel_Index int
)

func init() {
	fmt.Println("[hugh_db]")
	hugh_db_rul := "root:jinhyung@tcp(34.82.70.174:3306)/"
	hugh_db, hugh_db_err := sql.Open("mysql", hugh_db_rul+"nakama?parseTime=true")
	if hugh_db_err != nil {
		fmt.Println(hugh_db_err.Error())
	}
	fmt.Println(hugh_db_rul)

	DBManager = &runtime.DBManager{Hugh_db: hugh_db}

}
