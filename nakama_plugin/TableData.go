package main

//about response rode
const (
	Success         uint = 200
	BadRequest      uint = 400
	Unauthorized    uint = 401
	PaymentRequired uint = 402
	NotFound        uint = 404
	Conflict        uint = 409
	ServerError     uint = 500
)


//about user data
type UserData struct {
	Message     string `json:"message"`
	MessageCode uint   `json:"messageCode"`
	UserID    string `json:"userId"`
	UserName string `json:"userName"`
	UserLevel int    `json:"userLevel"`
	UserGold  int    `json:"userGold"`
}

type ReqSetUserPacket struct {
	UserId      string `json:"userId"`
	UserName string `json:"userName"`
	UserLevel   int    `json:"userLevel"`
	UserGold    int    `json:"userGold"`
}

type ReqUserInfoPacket struct {
	UserId      string `json:"userId"`
}
