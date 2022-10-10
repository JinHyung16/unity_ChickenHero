# GCP-nakama_login server setting

## Server-driven Environment
- GCP를 이용한 linux - ubuntu 18.04 LTS version

## Folder Structure
- server folder: /root 밑 server로 사용될 폴더, 각종 shell 스크립토로 만든 파일 존재  
- login-server folder: docker-compose, Dockerfile, nakama.so(서버 go builder file), 존재  
- login-server-DB folder: 로그인 시 정보 관리하는 mysql DB  

## USAGES
- 순서 중요하다
- 1) login-server-DB의 docker-compose up
- 2) login-server의 docker-compose up