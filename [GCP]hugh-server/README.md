# GCP-nakama server setting

## Server-driven Environment
- GCP를 이용한 linux - ubuntu 18.04 LTS version  

## Folder Structure  
- server folder: /root 밑 server로 사용될 폴더, 각종 shell 스크립토로 만든 파일 존재  
- hugh-server folder: docker-compose, Dockerfile, nakama.so(서버 go builder file), 존재 
- data folder: local.yml, logfile.txt 파일 존재 -> local.yml은 Dockerfile에서 읽고, logfile.txt에 각종 로그들 찍어놓음    
- modules folder: grpc를 이용해 custom한 nakama_plugin 폴더에서 (go mod name).so 파일을 뽑아 배치  

## Usages
- hugh-db 폴더 작업 시, mysql 서버 연다음 docker-compose up 실행시킨 상태 확인  
- 그 후, db폴더 하위에 있는 mysql관련 모든 폴더 및 파일 권한 777 부여하기  
- sudo chmod -R 777 db를 통해 db폴더 하위에 속한 모든 것에 권한 777 부여 가능  

## Notice
- dokcer-compose up 후 permission관련 에러가 생기면, 항상 server 하위에 존재한 모든 폴더와 파일 권한 777 주기  
- sudo chmod -R 777 server (-> server 폴더 하위 모든 파일과 폴더 권한 777로 변경 가능)  