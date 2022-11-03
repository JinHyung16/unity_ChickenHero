package Match

import (	
	"context"
	"database/sql"
	"encoding/json"
	"fmt"

	"HughCommon/Go/src/TableData"
	"github.com/heroiclabs/nakama-common/runtime"
)

const (
	MIN_PLAYER = 2
)

type Match struct {
}

type MatchState struct {
	Users map[string]runtime.Presence
}

func RegisterMatch(logger runtime.Logger, initializer runtime.Initializer) error {
	initializer.RegisterMatch("make_match", MakeMatch)

	fmt.Println("[RegisterMatch] : SUCCESS")
	return nil
}

func MakeMatch(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule) (runtime.Match, error) {
	return &Match{}, nil
}

func (m* Match)MatchInit(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, params map[string]interface{}) (interface{}, int, string) {
	m.Users = make(map[string]runtime.Presence)

	state := &Match{}
	tickRate := 2
	label := ""

	return state, tickRate, label
}

func (m* Match)MatchJoin(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, params map[string]interface{}) (interface{}, int, string) {
	
}

func (m* Match)MatchJoinAttempt(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, params map[string]interface{}) (interface{}, int, string) {

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

func (m *Match) MatchLoop(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, messages []runtime.MatchData) interface{} {
	lobbyState, ok := state.(*Match)
	if !ok {
		logger.Error("state not a valid lobby state object")
	}

	// If we have no presences in the match according to the match state, increment the empty ticks count
	if len(lobbyState.presences) == 0 {
		lobbyState.emptyTicks++
	}

	// If the match has been empty for more than 100 ticks, end the match by returning nil
	if lobbyState.emptyTicks > 100 {
		return nil
	}

	return lobbyState
}

func (m *Match) MatchTerminate(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, messages []runtime.MatchData) interface{} {
}

func (m *Match) MatchSignal(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, messages []runtime.MatchData) interface{} {
}
