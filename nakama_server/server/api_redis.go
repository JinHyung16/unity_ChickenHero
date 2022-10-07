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
		Addr:     "34.168.175.130:6379",
		Password: "jinhyung", // no password set
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
			//Addr:     "172.17.0.1:6379",
			Addr:     "34.168.175.130:6379",
			Password: "jinhyung", // no password set
			//DB:       0,              // use default DB
		})
		return rClient
	} else {
		return rClient
	}
}

//expire연장용 함수 필요할까?
func SetSessionCache(userid, username, sessionToken, refreshToken string, sessionExp, refreshExp, targetdbId int64, gameChannelId int64) {
	key := "session:" + userid
	err := redisClient().HMSet(ctx, key, map[string]interface{}{
		"userid":        userid,
		"username":      username,
		"sessionToken":  sessionToken,
		"refreshToken":  refreshToken,
		"sessionExp":    sessionExp,
		"refreshExp":    refreshExp,
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
		fmt.Println(key, " does not exist")
		return key, nil

	} else if err != nil {
		panic(err)

	} else {
		// for key, val := range sessionCache {
		// 	fmt.Println(key, val)
		// }
		return key, sessionCache
	}
}
