# Fabric-Agent-Test

Azure AI Foundry 기반 컴플라이언스 리포트 AI 에이전트

## 프로젝트 개요

Microsoft Fabric OneLake의 데이터를 기반으로 규정 위반 사항을 탐지하고, AI를 활용하여 컴플라이언스 리포트를 자동 생성하는 시스템입니다.

## 기술 스택

### Backend
- .NET 8.0 ASP.NET Core Web API
- Azure OpenAI (GPT-4o)
- Semantic Kernel
- Swagger/OpenAPI

### Frontend
- Blazor Server
- Markdown 렌더링 (Markdig)
- 실시간 스트리밍 채팅 UI

## 프로젝트 구조

```
Fabric-Agent-Test/
├── FabricAgent.sln
├── README.md
├── docs/
│   └── architecture.md
└── src/
    ├── FabricAgent.Backend/    # ASP.NET Core Web API
    │   ├── Program.cs
    │   ├── appsettings.json
    │   └── FabricAgent.Backend.csproj
    └── FabricAgent.Frontend/   # Blazor Server UI
        ├── Program.cs
        ├── Pages/
        │   ├── Index.razor      # 채팅 UI
        │   └── _Host.cshtml
        ├── Shared/
        │   └── MainLayout.razor
        └── FabricAgent.Frontend.csproj
```

## 시작하기

### 사전 요구사항

- .NET 8.0 SDK
- Azure OpenAI 계정 및 엔드포인트
- (선택) Microsoft Fabric 접근 권한

### 설정

1. **Backend 설정**

   `src/FabricAgent.Backend/appsettings.json` 파일에서 Azure AI 자격증명을 설정하세요:

   ```json
   {
     "AzureAI": {
       "ProjectEndpoint": "YOUR_AZURE_OPENAI_ENDPOINT",
       "DeploymentName": "gpt-4o",
       "ApiKey": "YOUR_API_KEY"
     }
   }
   ```

2. **Frontend 설정** (선택사항)

   `src/FabricAgent.Frontend/appsettings.json`에서 백엔드 URL을 변경할 수 있습니다:

   ```json
   {
     "BackendApi": {
       "BaseUrl": "http://localhost:5000"
     }
   }
   ```

### 실행 방법

#### 터미널 1: Backend 실행

```bash
cd src/FabricAgent.Backend
dotnet restore
dotnet run
```

Backend는 `http://localhost:5000`에서 실행됩니다.
Swagger UI: `http://localhost:5000/swagger`

#### 터미널 2: Frontend 실행

```bash
cd src/FabricAgent.Frontend
dotnet restore
dotnet run
```

Frontend는 `http://localhost:5001`에서 실행됩니다.

### 개발 모드 동시 실행

```bash
# 루트 디렉토리에서
dotnet build
```

## 주요 기능

### 1. AI 채팅 인터페이스
- Azure OpenAI GPT-4o 모델과의 실시간 스트리밍 대화
- Markdown 형식의 응답 렌더링
- 에러 처리 및 연결 상태 표시

### 2. Health Check
- Backend 상태 확인: `GET http://localhost:5000/health`

### 3. 확장 가능한 아키텍처
- Semantic Kernel 통합 준비
- Microsoft Fabric 데이터 연동 준비
- 플러그인 기반 스킬 시스템

## API 엔드포인트

### Backend API

- **Health Check**
  - `GET /health`
  - 응답: `{ "status": "healthy", "timestamp": "...", "service": "FabricAgent.Backend" }`

- **Chat**
  - `POST /api/agent/chat`
  - 요청: `{ "message": "질문 내용" }`
  - 응답: Server-Sent Events 스트림

## 개발 로드맵

- [x] 기본 Backend API 구조
- [x] Blazor Frontend UI
- [x] Azure OpenAI 통합
- [x] 스트리밍 채팅 기능
- [ ] Semantic Kernel 플러그인 구현
- [ ] Microsoft Fabric 데이터 연동
- [ ] 컴플라이언스 규정 마스터 데이터 관리
- [ ] 리포트 생성 및 PDF 내보내기

## 문제 해결

### Backend 연결 오류
- Backend가 `http://localhost:5000`에서 실행 중인지 확인
- `appsettings.json`의 Azure AI 자격증명이 올바른지 확인

### Azure OpenAI 오류
- API Key와 Endpoint가 유효한지 확인
- Deployment Name이 실제 배포된 모델 이름과 일치하는지 확인
- Azure Portal에서 할당량 및 권한 확인

## 라이센스

MIT License

## 기여

프로젝트 개선을 위한 제안이나 버그 리포트는 언제든 환영합니다.
