# GCP-DB server setting

## Folder Structure
- server folder: /root 밑에 생성한 폴더
- hugh-db folder: hugh-db 이름의 폴더 (현재 hugh-db 한개만 사용중) -> 추후 모듈러샤딩 기법 등 분산시 추가 가능
- /db/mysql : mysql server로 운용할 폴더 -> data, init folder 2개 존재해야함 (init folder는 empty folder)
- /data폴더 내 mysql 설정 + nakama db폴더 만들어 주기 (이떄 단순 생성이 아닌, mysql 환경 접속해서 만들어줘야 한다)


## [Beekeeper] Image Description
- Beekeeper studio를 통해 DB 테이블을 본 것
- DB_server connection: Host 부분엔 GCP 외부 IP를 입력, User와 Password 부분엔 docker-compose.yml에 작성한 값 입력
- nakama server 커스텀 하면서 server/core_authenticate.go 에서 필요한 table과 columns을 작성한 것
 