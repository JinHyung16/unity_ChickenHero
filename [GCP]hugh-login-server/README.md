# GCP_nakama_login server setting

## Server-driven Environment
- GCP를 이용한 linux - ubuntu 18.04 LTS version

## Folder Structure
- server folder: /root 밑 server로 사용될 폴더, 각종 shell 스크립토로 만든 파일 존재  
- login-server folder: docker-compose, Dockerfile, nakama.so(서버 go builder file), 존재  
- login-server-DB folder: 로그인 시 정보 관리하는 mysql DB  
- redis-server folder: redis 열어서 실행할 폴더  

## USAGES
- 모든 folder의 있는 docker-compose.yml 실행시켜야 한다.