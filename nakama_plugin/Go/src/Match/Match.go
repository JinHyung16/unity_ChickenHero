package Match

import (	
	"context"
	"database/sql"
	"fmt"

	"github.com/heroiclabs/nakama-common/runtime"
)

const (
	MIN_PLAYER = 2
)	

type Match struct {
	Users map[string]runtime.Presence
}

type MatchState struct {

}

func RegisterMatchMaking(logger runtime.Logger, initializer runtime.Initializer) error {
	initializer.RegisterMatch("make_match", MatchMaking)

	fmt.Println("[RegisterMatch] : SUCCESS")
	return nil
}

func MatchMaking(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule) (runtime.Match, error) {
	return &Match{}, nil
}

func (m* Match)MatchInit(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, params map[string]interface{}) (interface{}, int, string) {
	m.Users = make(map[string]runtime.Presence)

	state := &MatchState{}
	tickRate := 2
	label := ""

	return state, tickRate, label
}

func (m* Match)MatchJoinAttempt(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, presence runtime.Presence, metadata map[string]string) (interface{}, bool, string) {
	userId := ctx.Value(runtime.RUNTIME_CTX_USER_ID).(string)
	logger.Debug(userId)

	return state, true, " "
}

func (m* Match)MatchJoin(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, presences []runtime.Presence) interface{} {
	matchState, ok := state.(*MatchState)
	if !ok {
		logger.Error("state not a valid lobby state object")
	}

	for i := 0; i < len(presences); i++ {
		m.Users[presences[i].GetSessionId()] = presences[i]
	}

	return matchState
}

func (m *Match) MatchLoop(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, messages []runtime.MatchData) interface{} {
	matchState, ok := state.(*Match)
	if !ok {
		logger.Error("state not a valid lobby state object")
	}

	return matchState
}

func (m *Match) MatchLeave(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, presences []runtime.Presence) interface{} {
	myMatchState, ok := state.(*Match)
	if !ok {
		logger.Error("state not a valid lobby state object")
	}

	for i := 0; i < len(presences); i++ {
		delete(myMatchState.Users, presences[i].GetSessionId())
	}

	return myMatchState
}

func (m *Match) MatchTerminate(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, graceSeconds int) interface{} {
	return state
}


func (m *Match) MatchSignal(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, data string) (interface{}, string){
	return state, " "
}

