# unity_EggBreak  

## Version
- Nakama version : v3.13.1 
- Nakama Common version : v1.24.0  
- Nakama-Unity version : v3.5.0  
- Go version: 1.18 (all available within 1.18 version)  
- Unity version: 3.30f1 (use 3.9f1 or later)  

## Setting
- nakama_plugin folder: grpc 작성한 플러그인  
- nakama_server folder: nakama github에서 제공중인 api를 다운받아 redis를 추가하고 커스텀한 서버  
- nakama_server_login folder: nakama github에서 제공중인 api를 다운받아 migrate 수정하여 사용 

## Notice
- [GCP] 이름의 폴더는 GCP에서 운용중인 서버 환경 설정을 적어놨습니다.  

### Reference
- nakama server: github(https://github.com/heroiclabs/nakama)  
- [GCP]hugh-login-server의 Dockerfile 작성: github(https://github.com/heroiclabs/nakama/blob/master/build/Dockerfile)  
- nakama_login_server의 sql-migrate 참고: github(https://github.com/rubenv/sql-migrate/tree/v1.1.1)   