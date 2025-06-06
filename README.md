# Chicken Hero 🐔

Nakama 서버를 직접 구축하고, GCP 환경에서 배포 및 매치메이킹 시스템을 실험한 실전 네트워크 슈팅 게임입니다.  
멀티플레이 매칭/통신을 중심으로, MVP 구조 및 UniTask 기반 비동기 처리 등 구조 설계를 실현했습니다.

## 📌 프로젝트 개요

- 장르: 하이퍼 캐주얼 / 2D 멀티플레이 슈팅
- 기술 스택: Unity, C#, Nakama, GCP, UniTask
- 출시: Google Play 등록 완료
- 역할: 개인 개발 (기획 ~ 구현 ~ 배포)

## 🔧 주요 기능

- Nakama + GCP + Docker 서버 구축
- 매치메이킹 / 유저 위치/점수 동기화
- MVP 구조 및 옵저버 패턴 도입
- UniTask 기반 비동기 처리

## 💡 기술 포인트

- 실시간 매칭 및 유저 상태 연결 흐름 설계
- Coroutine → UniTask로 전환하며 GC 감소 및 구조 안정

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
