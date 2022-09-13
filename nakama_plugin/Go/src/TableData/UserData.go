package TableData

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
