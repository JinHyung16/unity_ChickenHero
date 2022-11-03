# unity_EggBreak  

## Version
- Nakama version : v3.13.1 
- Nakama Common version : v1.24.0  
- Nakama-Unity version : v3.5.0  
- Go version: 1.18 (all available within 1.18 version)  
- Unity version: 3.30f1  

## Setting
- nakama_plugin folder: grpc 작성한 플러그인  
- nakama_server folder: nakama github에서 제공중인 api를 다운받아 redis를 추가하고 커스텀한 서버  
- nakama_server_login folder: nakama github에서 제공중인 api를 다운받아 migrate 수정하여 사용 

## Notice
- [GCP] 이름의 폴더는 GCP에서 운용중인 서버 환경 설정을 적어놨습니다.  
- nakama github에서 다운받아 사용한 nakama서버는 그대로 사용한 것이 아닌, 필요에 맞게 커스텀한 것입니다.  
- nakama github에서 nakama_master 다운받아 사용하면, nakama-common 버전이 달라 오류가 생길 수 있으니, 꼭 필요한 버전에 맞춰 tag에서 찾아 다운받아 커스텀하는거 추천  



### Reference
- nakama server: github(https://github.com/heroiclabs/nakama)  
- [GCP]hugh-login-server의 Dockerfile 작성: github(https://github.com/heroiclabs/nakama/blob/master/build/Dockerfile)  
- nakama_server의 sql-migrate 참고: github(https://github.com/rubenv/sql-migrate/tree/v1.1.1)   