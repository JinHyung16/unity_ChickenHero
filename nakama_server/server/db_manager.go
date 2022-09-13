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
)

func init() {
	//TODO : nakama 실행시 user_db_url을 os.Args로 넣어줘서 설정할수 있게.. or admin 서버의 db에서 읽어오게

	//ctx := context.Background()
	/*
	var db_err error
	user_db_url := "root:Nurhym6398!@@tcp(34.64.137.24:3306)/" //login-db
	user_db, db_err := sql.Open("mysql", user_db_url+"nakama?parseTime=true")
	if db_err != nil {
		fmt.Println(db_err.Error())
	}
	*/
	//defer user_db.Close()

	///Hugh db
	//
	//
	fmt.Println("[hugh_db]")
	hugh_db_rul := "root:jinhyung@tcp(34.82.70.174:3306)/"
	hugh_db, hugh_db_err := sql.Open("mysql", hugh_db_rul+"nakama?parseTime=true")
	if hugh_db_err != nil {
		fmt.Println(hugh_db_err.Error())
	}
	fmt.Println(hugh_db_rul)
	///////////////////

	DBManager = &runtime.DBManager{Hugh_db: hugh_db}

}
