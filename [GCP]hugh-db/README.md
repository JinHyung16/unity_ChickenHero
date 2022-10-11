# GCP-DB server setting

## Folder Structure
- server folder: /root 밑에 생성한 폴더  
- /db/mysql : mysql server로 운용할 폴더 -> data, init folder 2개 존재해야함 (init folder는 empty folder)  

## Usages
- docker-compose up 실행하여 mysql 서버 열고 만든다음 db 폴더/mysql 폴더/data 폴더 까지 접근  
- docker exec -it mysql컨테이너이름 /bin/bash (->container bash 접근)  
- mysql -u root -p (-> container bash에서 mysql 서버 접근)  
- create database 폴더명 (-> mysql db담을 폴더 생성)  
- docker-compose.yml이 존재하는 폴더까지 올라간 후, docker-compose up실행  
- db 폴더 상위까지 접근 한 후 sudo chmod -R 777 db 실행  

## Notice
- 'nakama' 이름의 database를 추가하여 거기서 table을 만들어 사용  
- GCP 방화벽에서 3307 port도 열어서 사용  
- 항상 docker-compose up 이전에 server폴더 하위 모든 파일 및 폴더 권한 777로 바꿔주기  
- sudo chmod -R 777 server (-> server폴더 하위 모든 파일 및 폴더 권한 777로 변경 가능)  