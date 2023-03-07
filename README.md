# unity_ChickenHero  

## Version
- Nakama version : v3.14.0 
- Nakama Common version : v1.25.0  
- Nakama-Unity version : v3.5.0  
- Go version: 1.18 (all available within 1.18 version)  
- Unity version: 21.3.18f1  
- UniTask version 2.3.3

## Setting
- [GCP]hugh-db folder: mysql DB 서버용, grpc에서 읽고 쓸 DB서버  
- [GCP]hugh-server folder: Game server용
- nakama_plugin folder: grpc 작성한 플러그인  
- nakama_server folder: 커스텀한 nakama server  

## Notice
- [GCP] 이름의 폴더는 GCP에서 운용중인 서버 환경 설정을 적어놨습니다.  
- nakama server의 경우, 공식 nakama github에서 v3.14.0 다운받아 해당 서버용 mysql migrate위해 커스텀 했습니다.    
