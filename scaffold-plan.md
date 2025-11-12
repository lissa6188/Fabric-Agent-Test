# Scaffold Plan

이 문서는 로컬에서 첫 번째 MVP 스캐폴딩을 만드는 방법을 설명합니다.

1) 선택: Blazor Server (frontend) + ASP.NET Core (backend)

2) 간단한 명령 (로컬에서 dotnet SDK가 설치된 경우)

```bash
cd /workspaces/Fabric-Agent-Test
dotnet new sln -n FabricAgent
mkdir src && cd src
dotnet new blazorserver -n FabricAgent.Frontend
dotnet new webapi -n FabricAgent.Backend
dotnet sln ../FabricAgent.sln add FabricAgent.Frontend/FabricAgent.Frontend.csproj
dotnet sln ../FabricAgent.sln add FabricAgent.Backend/FabricAgent.Backend.csproj
```

3) 기본 연결

- Backend: `/api/agent/generate` 엔드포인트 추가(POST: {sessionId, prompt}) → Semantic Kernel + Foundry 호출
- Frontend: `HttpClient` 또는 SignalR을 통해 `/api/agent/generate` 호출, 진행 상태 표시

4) 환경변수(예시)

- AZURE_FOUNDRY_ENDPOINT
- AZURE_TENANT_ID
- AZURE_CLIENT_ID
- AZURE_CLIENT_SECRET (또는 Managed Identity 사용)

5) 검증 시나리오

- 프론트에서 간단한 prompt 전송 → 백엔드가 모의 Foundry 응답(더미) 반환 → 프론트가 결과 렌더링

이제 원하시면 실제 `dotnet new` 명령으로 프로젝트를 생성하거나, 제가 파일들을 직접 생성해 작은 샘플을 구현해 드리겠습니다.
