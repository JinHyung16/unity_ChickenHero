package server

import (
	"context"
	"fmt"
	"time"

	"github.com/go-redis/redis/v8"
)

var (
	ctx               = context.Background()
	rClient           *redis.Client
	sessionExpireTime = 7200 * time.Second
	//exp := time.Now().UTC().Add(time.Duration(config.GetSession().TokenExpirySec) * time.Second).Unix()
)

func ExampleClient() {
	rdb := redis.NewClient(&redis.Options{
		//Addr:     "172.17.0.1:6379",
		Addr:     "34.168.124.155:6379", //GCP hugh-login-server 외부 IP
		Password: "jinhyung",
		//DB:       0,              // use default DB
	})

	err := rdb.Set(ctx, "hugh", "test", 0).Err()
	if err != nil {
		panic(err)
	}

	val, err := rdb.Get(ctx, "hugh").Result()
	if err != nil {
		panic(err)
	}
	fmt.Println("hugh", val)

	val2, err := rdb.Get(ctx, "hugh2").Result()
	if err == redis.Nil {
		fmt.Println("hugh2 does not exist")
	} else if err != nil {
		panic(err)
	} else {
		fmt.Println("hugh2", val2)
	}
	// Output: key value
	// key2 does not exist
}

func redisClient() *redis.Client {
	if rClient == nil {
		rClient = redis.NewClient(&redis.Options{
			//Addr:     "172.17.0.1:6379", // 같은 VM에 있을시 ip
			Addr:     "34.168.124.155:6379", //GCP hugh-login-server 외부 IP
			Password: "jinhyung",
			//DB:       0,              // use default DB
		})
		return rClient
	} else {
		return rClient
	}
}

func SetSessionCache(userid, username, sessionToken, refreshToken string, sessionExp, refreshExp, targetdbId int64) {
	key := "session:" + userid
	err := redisClient().HMSet(ctx, key, map[string]interface{}{
		"userid":        userid,
		"username":      username,
		"sessionToken":  sessionToken,
		"refreshToken":  refreshToken,
		"sessionExp":    sessionExp,
		"refreshExp":    refreshExp,
		"targetdbId":   targetdbId,
	}).Err()

	if err != nil {
		panic(err)
	} else {
		expireErr := redisClient().Expire(ctx, key, sessionExpireTime).Err()
		if expireErr != nil {
			panic(expireErr)
		}
	}
}

func GetSessionCache(userid string) (string, map[string]string) {
	key := "session:" + userid
	sessionCache, err := redisClient().HGetAll(ctx, key).Result()
	if err == redis.Nil || len(sessionCache) == 0 {
		fmt.Println("[api_redis.go] 89 : does not exist key",key)
		return key, nil

	} else if err != nil {
		fmt.Println("[api_redis.go] 93")
		panic(err)

	} else {
		/*
		 for key, val := range sessionCache {
		 	fmt.Println(key, val)
		 }
		 */
		fmt.Println("[api_redis.go] 101")
		return key, sessionCache
	}
}

//세션 종료시 호출
func DeleteSessionCache(userId string) {
	key := "session:" + userId
	fmt.Println("[api_redis.go] 107 : DeleteSessionCache :" + key)
	redisClient().Del(ctx, key)
}

func DeleteAllSessionCache() {
	fmt.Println("[api_redis.go] 111 : DeleteAllSessionCache")
	redisClient().FlushAll(ctx)
}