# GCP_nakama server setting

## Server-driven Environment
- GCP를 이용한 linux - ubuntu 18.04 LTS version  

## Folder Structure  
- server folder: /root 밑 server로 사용될 폴더, 각종 shell 스크립토로 만든 파일 존재  
- hugh-server folder: docker-compose, Dockerfile, nakama.so(서버 go builder file), 존재 
- data folder: local.yml, logfile.txt 파일 존재 -> local.yml은 Dockerfile에서 읽고, logfile.txt에 각종 로그들 찍어놓음    
- modules folder: grpc를 이용해 custom한 nakama_plugin 폴더에서 (go mod name).so 파일을 뽑아 배치  