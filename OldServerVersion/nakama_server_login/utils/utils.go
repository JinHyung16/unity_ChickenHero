package utils

import (
	"bytes"
	"encoding/gob"
	"encoding/json"
	"log"
	"net"
	"net/http"
	"strings"
)

func ToBytes(data interface{}) []byte {
	var bufferdata bytes.Buffer
	encoder := gob.NewEncoder(&bufferdata)
	encoder.Encode(encoder.Encode(data))
	return bufferdata.Bytes()
}

func HandleError(err error) {
	if err != nil {
		log.Panic(err)
	}
}

func Splitter(s string, sep string, i int) string {
	r := strings.Split(s, sep)
	if len(r)-1 < i {
		return ""
	}
	return r[i]
}

func ToJson(i interface{}) []byte {
	jsonData, err := json.Marshal(i)
	HandleError(err)
	return jsonData
}

func ExtractClientAddressFromRequest(r *http.Request) (string, string) {
	var clientAddr string
	if ips := r.Header.Get("x-forwarded-for"); len(ips) > 0 {
		clientAddr = strings.Split(ips, ",")[0]
	} else {
		clientAddr = r.RemoteAddr
	}

	return extractClientAddress(clientAddr)
}

func extractClientAddress(clientAddr string) (string, string) {
	var clientIP, clientPort string

	if clientAddr != "" {

		clientAddr = strings.TrimSpace(clientAddr)
		if host, port, err := net.SplitHostPort(clientAddr); err == nil {
			clientIP = host
			clientPort = port
		} else if addrErr, ok := err.(*net.AddrError); ok {
			switch addrErr.Err {
			case "missing port in address":
				fallthrough
			case "too many colons in address":
				clientIP = clientAddr
			default:
			}
		}
	}

	return clientIP, clientPort
}
