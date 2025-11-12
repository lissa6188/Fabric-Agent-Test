# Fabric-Agent-Test

# Azure AI Compliance Agent (Fabric + Semantic Kernel)

온도 센서·근무시간·점검 로그 같은 **팩트 데이터**를 기반으로,  
규정 위반 탐지 결과를 사람이 바로 제출 가능한 **컴플라이언스 리포트**로 자동 생성하는 데모 프로젝트입니다.

- **데이터:** Microsoft Fabric OneLake (Lakehouse)  
- **분석:** Fabric(노트북 / SQL / ML)에서 집계·이상탐지  
- **AI 계층:** Azure AI Foundry + Semantic Kernel 기반 LLM 에이전트  
- **앱:** 로컬 웹 앱 (리포트 조회 + 프롬프트 실행)

핵심 개념은 다음과 같습니다:  
> “탐지는 규칙이나 ML이 하고, 설명·요약·보고서는 LLM이 한다.”

---

## 1. 시나리오 요약

이 프로젝트는 다음과 같은 데이터 흐름을 가정합니다.

1. 설비 온도, 작업시간 등의 로그가 Fabric Lakehouse에 적재됩니다.  
2. 규정 마스터(예: REG-S003 온도 초과, REG-S005 근로시간 초과)와 조인해 **위반/위험 이벤트**를 계산합니다.  
3. Azure AI Foundry + Semantic Kernel로 만든 **Compliance Report Agent**가:
   - OneLake/Fabric에서 집계 쿼리 실행  
   - 규정별/라인별/기간별 위반 현황을 표와 텍스트로 생성  
   - 차트 구성을 위한 JSON 시각화 스펙까지 반환  
4. 로컬 웹 앱은 이를 받아 리포트를 렌더링하고 PDF/파일로 내보낼 수 있습니다.

핵심은 LLM이 **탐지 결과를 해석·요약·문서화하는 역할**을 맡는 것입니다.

---

## 2. 아키텍처

[Sensor / Log Data]
|
v
[Fabric Lakehouse / Warehouse]
|
| (SQL / Notebook / ML 집계 및 이상탐지)
v
[Aggregated Views / Materialized Tables]
|
| (Semantic Kernel connector)
v
[Azure AI Foundry - LLM / Agent]
|
| (REST API)
v
[Local Web App] --> 사용자 조회, 프롬프트 실행, 리포트 생성



**핵심 포인트**
- 데이터의 진실은 Fabric에 있고,  
- LLM은 쿼리 생성, 결과 해석, 보고서 작성만 담당합니다.  
- Semantic Kernel은 Fabric SQL 호출 플러그인과 리포트 포매팅 플러그인을 통해 이를 제어합니다.

---

## 3. 주요 기능

### ① Rule-aware Reporting  
- `rule_master.csv`를 기준으로 규정 메타데이터 로드  
- 센싱 데이터·근무시간·정비 이력과 조합해 위반 현황 생성  

### ② AI Generated Compliance Report  
- 기간, 라인, 설비를 입력하면:  
  - LLM이 SQL 생성 → Fabric SQL 실행  
  - 결과를 요약문, 표, 시각화 스펙으로 변환  
- 반환 구조:
  ```json
  {
    "summary": "...",
    "tables": [...],
    "charts": [...]
  }


### ③ Local Web UI

- 기간/라인 선택 → “Generate Report” 클릭
- 백엔드가 SK + Azure AI 호출
- 결과를 HTML 테이블·차트로 렌더링 (PDF 내보내기 가능)

## 4. 리포지토리 구조 예시



## 5. 환경

- Microsoft Fabric (Lakehouse)
- Azure 구독
- Azure AI Foundry (프로젝트/모델 엔드포인트)
- Semantic Kernel (C# 또는 Python SDK)

---

## 프론트엔드 선택 (결정)

이 프로젝트는 Microsoft 생태계 중심으로 운영되므로, 프론트엔드를 **Blazor (C#/.NET)** 으로 진행하기로 결정했습니다.

권장 모드: **Blazor Server** — 서버측에서 .NET과 직접 통합되며, SignalR 연결을 통해 스트리밍/실시간 상태 관리가 수월합니다. Semantic Kernel(.NET)과의 통합, Azure AD/Managed Identity 사용이 자연스럽기 때문에 초기 MVP와 Azure 네이티브 통합을 빠르게 만들기 좋습니다.

원하시면 React(TypeScript)로도 별도 브랜치를 만들어 병행할 수 있습니다.

## 다음 단계

1. [완료] `src/FabricAgent.Frontend` — Blazor Server 앱 스캐폴딩 완료
2. [완료] `src/FabricAgent.Backend` — ASP.NET Core Web API 템플릿 생성 완료
3. [진행 중] Semantic Kernel + Azure Foundry SDK 설치 및 핵심 엔드포인트 구현
4. [예정] 간단한 E2E 테스트 — 프론트에서 'Generate Report' → 백엔드 Agent 호출 → 결과 렌더링

## 현재 프로젝트 구조

```
Fabric-Agent-Test/
├── FabricAgent.sln
├── docs/
│   └── architecture.md
├── scaffold-plan.md
└── src/
    ├── FabricAgent.Backend/    # ASP.NET Core Web API + Agent Orchestrator
    └── FabricAgent.Frontend/   # Blazor Server UI
```

