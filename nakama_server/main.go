// Copyright 2018 The Nakama Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

package main

import (
	"context"
	"fmt"
	"math/rand"
	"net/http"
	"os"
	"os/signal"
	"runtime"
	//"strconv"
	"syscall"
	"time"

	"google.golang.org/protobuf/encoding/protojson"

	"io/ioutil"
	"path/filepath"

	_ "github.com/go-sql-driver/mysql"
	"github.com/gofrs/uuid"
	"github.com/heroiclabs/nakama/v3/server"
	"github.com/heroiclabs/nakama/v3/social"
	_ "github.com/jackc/pgx/v4/stdlib"
	"go.uber.org/zap"
	"go.uber.org/zap/zapcore"
)

const cookieFilename = ".cookie"

var (
	version  string = "3.0.0"
	commitID string = "dev"

	// Shared utility components.
	jsonpbMarshaler = &protojson.MarshalOptions{
		UseEnumNumbers:  true,
		EmitUnpopulated: false,
		Indent:          "",
		UseProtoNames:   true,
	}
	jsonpbUnmarshaler = &protojson.UnmarshalOptions{
		DiscardUnknown: false,
	}

	//user_contents_db map[int]*sql.DB
	//user_db          *sql.DB
	//Test             string = "ok"
)

func main() {
	semver := fmt.Sprintf("%s+%s", version, commitID)
	// Always set default timeout on HTTP client.
	http.DefaultClient.Timeout = 1500 * time.Millisecond
	// Initialize the global random obj with customs seed.
	rand.Seed(time.Now().UnixNano())

	tmpLogger := server.NewJSONLogger(os.Stdout, zapcore.InfoLevel, server.JSONFormat)

	if len(os.Args) > 1 {
		switch os.Args[1] {
		case "--version":
			fmt.Println(semver)
			return
		}
	}

	config := server.ParseArgs(tmpLogger, os.Args)
	logger, startupLogger := server.SetupLogging(tmpLogger, config)
	// configWarnings := server.CheckConfig(logger, config)
	server.CheckConfig(logger, config)

	startupLogger.Info("Nakama(게임) starting")
	startupLogger.Info("Node", zap.String("name", config.GetName()), zap.String("version", semver), zap.String("runtime", runtime.Version()), zap.Int("cpu", runtime.NumCPU()), zap.Int("proc", runtime.GOMAXPROCS(0)))
	startupLogger.Info("Data directory", zap.String("path", config.GetDataDir()))
	startupLogger.Info("MaxMessageSizeBytes : ", zap.Int64("MaxMessageSizeBytes", config.GetSocket().MaxMessageSizeBytes))
	startupLogger.Info("ReadBufferSizeBytes : ", zap.Int64("MaxMessageSizeBytes", int64(config.GetSocket().ReadBufferSizeBytes)))
	startupLogger.Info("WriteBufferSizeBytes : ", zap.Int64("MaxMessageSizeBytes", int64(config.GetSocket().WriteBufferSizeBytes)))

	// Global server context.
	ctx, ctxCancelFn := context.WithCancel(context.Background())

	// Check migration status and fail fast if the schema has diverged.
	// migrate.StartupCheck(startupLogger, db)

	// Access to social provider integrations.
	socialClient := social.NewClient(logger, 5*time.Second)

	// Start up server components.
	// cookie := newOrLoadCookie(config)
	// metrics := server.NewMetrics(logger, startupLogger, db, config)
	sessionRegistry := server.NewLocalSessionRegistry( /*metrics*/ )
	sessionCache := server.NewLocalSessionCache(config)
	statusRegistry := server.NewStatusRegistry(logger, config, sessionRegistry, jsonpbMarshaler)
	tracker := server.StartLocalTracker(logger, config, sessionRegistry, statusRegistry /*metrics,*/, jsonpbMarshaler)
	router := server.NewLocalMessageRouter(sessionRegistry, tracker, jsonpbMarshaler)
	// leaderboardCache := server.NewLocalLeaderboardCache(logger, startupLogger, db)
	// leaderboardRankCache := server.NewLocalLeaderboardRankCache(startupLogger, db, config.GetLeaderboard(), leaderboardCache)
	// leaderboardScheduler := server.NewLocalLeaderboardScheduler(logger, db, config, leaderboardCache, leaderboardRankCache)
	matchRegistry := server.NewLocalMatchRegistry(logger, startupLogger, config, sessionRegistry, tracker, router /*metrics,*/, config.GetName())
	tracker.SetMatchJoinListener(matchRegistry.Join)
	tracker.SetMatchLeaveListener(matchRegistry.Leave)
	streamManager := server.NewLocalStreamManager(config, sessionRegistry, tracker)
	// runtime, runtimeInfo, err := server.NewRuntime(ctx, logger, startupLogger, db, jsonpbMarshaler, jsonpbUnmarshaler, config, socialClient /*leaderboardCache, leaderboardRankCache, leaderboardScheduler,*/, sessionRegistry, sessionCache, matchRegistry, tracker /*metrics,*/, streamManager, router)
	//runtime, _, err := server.NewRuntime(ctx, logger, startupLogger, user_contents_db, jsonpbMarshaler, jsonpbUnmarshaler, config, socialClient /*leaderboardCache, leaderboardRankCache, leaderboardScheduler,*/, sessionRegistry, sessionCache, matchRegistry, tracker /*metrics,*/, streamManager, router)
	runtime, _, err := server.NewRuntime(ctx, logger, startupLogger, server.DBManager, jsonpbMarshaler, jsonpbUnmarshaler, config, socialClient /*leaderboardCache, leaderboardRankCache, leaderboardScheduler,*/, sessionRegistry, sessionCache, matchRegistry, tracker /*metrics,*/, streamManager, router)

	if err != nil {
		startupLogger.Fatal("Failed initializing runtime modules", zap.Error(err))
	}
	matchmaker := server.NewLocalMatchmaker(logger, startupLogger, config, router, runtime)
	partyRegistry := server.NewLocalPartyRegistry(logger, matchmaker, tracker, streamManager, router, config.GetName())
	tracker.SetPartyJoinListener(partyRegistry.Join)
	tracker.SetPartyLeaveListener(partyRegistry.Leave)

	// leaderboardScheduler.Start(runtime)

	//pipeline := server.NewPipeline(logger, config, user_contents_db, jsonpbMarshaler, jsonpbUnmarshaler, sessionRegistry, statusRegistry, matchRegistry, partyRegistry, matchmaker, tracker, router, runtime)
	pipeline := server.NewPipeline(logger, config, server.DBManager, jsonpbMarshaler, jsonpbUnmarshaler, sessionRegistry, statusRegistry, matchRegistry, partyRegistry, matchmaker, tracker, router, runtime)
	// statusHandler := server.NewLocalStatusHandler(logger, sessionRegistry, matchRegistry, tracker /*metrics,*/, config.GetName())

	//apiServer := server.StartApiServer(logger, startupLogger, user_contents_db, jsonpbMarshaler, jsonpbUnmarshaler, config, socialClient /*leaderboardCache, leaderboardRankCache,*/, sessionRegistry, sessionCache, statusRegistry, matchRegistry, matchmaker, tracker, router /*metrics,*/, pipeline, runtime)
	apiServer := server.StartApiServer(logger, startupLogger, server.DBManager, jsonpbMarshaler, jsonpbUnmarshaler, config, socialClient /*leaderboardCache, leaderboardRankCache,*/, sessionRegistry, sessionCache, statusRegistry, matchRegistry, matchmaker, tracker, router /*metrics,*/, pipeline, runtime)
	// consoleServer := server.StartConsoleServer(logger, startupLogger, db, config, tracker, router, sessionCache, statusHandler, runtimeInfo, matchRegistry, configWarnings, semver /*leaderboardCache, leaderboardRankCache,*/, apiServer, cookie)

	// Respect OS stop signals.
	c := make(chan os.Signal, 2)
	signal.Notify(c, os.Interrupt, syscall.SIGINT, syscall.SIGTERM)

	startupLogger.Info("Startup done")

	// Wait for a termination signal.
	<-c

	graceSeconds := config.GetShutdownGraceSec()

	// If a shutdown grace period is allowed, prepare a timer.
	var timer *time.Timer
	timerCh := make(<-chan time.Time, 1)
	if graceSeconds != 0 {
		timer = time.NewTimer(time.Duration(graceSeconds) * time.Second)
		timerCh = timer.C
		startupLogger.Info("Shutdown started - use CTRL^C to force stop server", zap.Int("grace_period_sec", graceSeconds))
	} else {
		// No grace period.
		startupLogger.Info("Shutdown started")
	}

	// Stop any running authoritative matches and do not accept any new ones.
	select {
	case <-matchRegistry.Stop(graceSeconds):
		// Graceful shutdown has completed.
		startupLogger.Info("All authoritative matches stopped")
	case <-timerCh:
		// Timer has expired, terminate matches immediately.
		startupLogger.Info("Shutdown grace period expired")
		<-matchRegistry.Stop(0)
	case <-c:
		// A second interrupt has been received.
		startupLogger.Info("Skipping graceful shutdown")
		<-matchRegistry.Stop(0)
	}
	if timer != nil {
		timer.Stop()
	}

	// Signal cancellation to the global runtime context.
	ctxCancelFn()

	// Gracefully stop remaining server components.
	apiServer.Stop()
	// consoleServer.Stop()
	// metrics.Stop(logger)
	matchmaker.Stop()
	// leaderboardScheduler.Stop()
	tracker.Stop()
	statusRegistry.Stop()
	sessionCache.Stop()
	sessionRegistry.Stop()

	startupLogger.Info("Shutdown complete")

	os.Exit(0)
}


func newOrLoadCookie(config server.Config) string {
	filePath := filepath.FromSlash(config.GetDataDir() + "/" + cookieFilename)
	b, err := ioutil.ReadFile(filePath)
	cookie := uuid.FromBytesOrNil(b)
	if err != nil || cookie == uuid.Nil {
		cookie = uuid.Must(uuid.NewV4())
		_ = ioutil.WriteFile(filePath, cookie.Bytes(), 0644)
	}
	return cookie.String()
}
