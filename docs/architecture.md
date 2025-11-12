# Architecture Overview

이 문서는 Azure AI Foundry + Semantic Kernel(.NET) 기반의 Compliance Agent 앱 아키텍처를 요약합니다.

## 구성 요소

- Frontend (Blazor Server): 사용자 프롬프트, 파일 업로드, 리포트 렌더링, 인증(UI에서 Azure AD 로그인/SSO 연동).
- Backend (ASP.NET Core): Agent Orchestrator, Semantic Kernel(.NET) 통합, Azure Foundry 호출 어댑터, 플러그인(검색, 파일, 외부 API).
- Data: Microsoft Fabric OneLake (Lakehouse) — 집계/머티리얼라이즈된 뷰를 사용.
- Auth: Azure AD / Managed Identity — 백엔드가 Foundry 호출 시 Managed Identity 또는 서비스 주체 사용 권장.

## 데이터 흐름 (간단)

1. 사용자가 프론트에서 리포트 요청(기간, 라인 등) 전송
2. 백엔드가 세션/메모리(요약/컨텍스트)를 로드
3. Semantic Kernel가 도구(예: Fabric SQL 커넥터)를 사용해 필요한 데이터를 조회하거나, Azure Foundry Agent에 플래닝을 요청
4. Azure Foundry가 모델 응답(요약/포맷)을 반환
5. 백엔드가 포맷을 정리해 프론트로 전송(Streaming 가능)

## 플러그인/스킬 설계(권장)

- SearchSkill: 웹검색/문서검색
- FabricSqlSkill: Fabric SQL 생성/실행
- FileSkill: 업로드된 파일 열람/요약
- CalendarSkill (옵션): 캘린더 읽기/쓰기

각 스킬은 명확한 입력/출력 계약(JSON)과 실패 모드(타임아웃, HTTP 오류)를 정의해야 합니다.

## 운영/모니터링

- 요청 로깅(입력 프롬프트, 호출 비용, 응답 시간)
- 레이트 제한/큐잉(비용 제어용)
- 장기 메모리 요약 및 TTL 정책

---

자세한 구현 계획은 `scaffold-plan.md`를 참조하세요.
