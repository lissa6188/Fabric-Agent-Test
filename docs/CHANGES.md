# 프로젝트 정리 및 수정 내역

## 수정 일자
2025년 11월 12일

## 수정 개요
LLM 모델을 여러 번 바꾸면서 발생한 소스 코드 문제들을 전체적으로 검토하고 수정했습니다.

## 주요 수정 사항

### 1. Backend (FabricAgent.Backend)

#### Program.cs
**수정 내용:**
- Azure OpenAI 클라이언트 초기화 방식 개선
  - `AzureKeyCredential` → `ApiKeyCredential`로 변경 (베타 SDK 호환)
  - Configuration 의존성 주입 방식으로 변경
- 에러 처리 강화
  - 빈 메시지 검증 추가
  - `RequestFailedException` → `ClientResultException`으로 변경
  - 상세한 로깅 추가
- 스트리밍 응답 개선
  - `[DONE]` 및 `[ERROR]` 마커 추가
  - 개행문자 이스케이프 처리
  - Cache-Control 헤더 추가
- CORS 설정 구체화
  - AllowAnyOrigin → 특정 오리진만 허용 (localhost:5001)

#### FabricAgent.Backend.csproj
**수정 내용:**
- 패키지 버전 최적화
  - `Azure.AI.Projects` 제거 (불필요)
  - `Azure.AI.OpenAI` 2.1.0-beta.1 (Semantic Kernel 호환 버전)
  - `Microsoft.SemanticKernel` 1.28.0 유지
  - `Swashbuckle.AspNetCore` 6.8.1로 업데이트

#### appsettings.json
**수정 내용:**
- 민감 정보 제거 (API Key, Endpoint)
- 플레이스홀더로 교체
- 로깅 레벨 구조 개선
- `ModelName` 불필요 항목 제거

### 2. Frontend (FabricAgent.Frontend)

#### Pages/Index.razor
**수정 내용:**
- HttpClient 주입 방식 변경
  - 직접 `HttpClient` 주입 → `IHttpClientFactory` 사용
- API 엔드포인트 설정 개선
  - 하드코딩된 URL → Configuration 기반으로 변경
- 스트리밍 응답 처리 개선
  - `[DONE]` 및 `[ERROR]` 마커 처리 추가
  - 이스케이프된 개행문자 복원
- 에러 처리 강화
  - `HttpRequestException` 별도 처리
  - 백엔드 연결 오류 명확한 메시지
- using 구문 정리
  - 불필요한 using 제거
  - `Microsoft.JSInterop` 추가

#### Program.cs
**수정 내용:**
- 불필요한 주석 제거
- 코드 간소화
- using 구문 정리

#### appsettings.json
**수정 내용:**
- Backend API 설정 추가
  ```json
  "BackendApi": {
    "BaseUrl": "http://localhost:5000"
  }
  ```

#### Properties/launchSettings.json
**수정 내용:**
- 포트 표준화 (5001로 통일)
- HTTPS 프로필 제거 (개발 환경 단순화)
- 불필요한 설정 제거

### 3. 문서화

#### README.md
**새로 작성:**
- 프로젝트 개요
- 기술 스택 명시
- 상세한 설정 가이드
- 실행 방법 (Backend/Frontend 분리)
- API 엔드포인트 문서화
- 개발 로드맵
- 문제 해결 가이드

#### docs/SETUP.md
**새로 생성:**
- Azure OpenAI 설정 단계별 가이드
- Microsoft Fabric 설정 가이드
- 환경별 설정 방법
- User Secrets 사용법
- 상세한 문제 해결 가이드

### 4. 패키지 버전 호환성

**해결된 문제:**
- Semantic Kernel과 Azure.AI.OpenAI 버전 충돌
- OpenAI SDK 버전 불일치

**최종 버전:**
- `Azure.AI.OpenAI`: 2.1.0-beta.1
- `Microsoft.SemanticKernel`: 1.28.0
- `Markdig`: 0.43.0
- `Swashbuckle.AspNetCore`: 6.8.1

## 빌드 검증

```bash
dotnet restore
dotnet build
```

결과: ✅ Build succeeded (0 Warning, 0 Error)

## 실행 포트

- **Backend**: http://localhost:5000
  - Swagger UI: http://localhost:5000/swagger
  - Health Check: http://localhost:5000/health
- **Frontend**: http://localhost:5001

## 보안 개선

1. API Key 및 민감 정보 제거
2. CORS 정책 구체화
3. User Secrets 사용 가이드 추가
4. 환경별 설정 분리 방법 문서화

## 다음 단계

### 즉시 필요한 작업
1. `src/FabricAgent.Backend/appsettings.json`에 Azure AI 자격증명 입력
   - ProjectEndpoint
   - ApiKey
   - DeploymentName

### 향후 개발 계획
1. Semantic Kernel 플러그인 구현
2. Microsoft Fabric 데이터 연동
3. 컴플라이언스 규정 마스터 데이터 관리
4. 리포트 생성 및 PDF 내보내기

## 변경된 파일 목록

```
수정:
- src/FabricAgent.Backend/Program.cs
- src/FabricAgent.Backend/FabricAgent.Backend.csproj
- src/FabricAgent.Backend/appsettings.json
- src/FabricAgent.Frontend/Pages/Index.razor
- src/FabricAgent.Frontend/Program.cs
- src/FabricAgent.Frontend/appsettings.json
- src/FabricAgent.Frontend/Properties/launchSettings.json
- README.md

생성:
- docs/SETUP.md
- docs/CHANGES.md (이 파일)

백업:
- README_OLD.md (기존 README)
```

## 테스트 체크리스트

- [x] dotnet restore 성공
- [x] dotnet build 성공 (0 경고, 0 오류)
- [ ] Backend 실행 테스트 (Azure AI 자격증명 필요)
- [ ] Frontend 실행 테스트
- [ ] E2E 채팅 기능 테스트
- [ ] 스트리밍 응답 테스트
- [ ] 에러 처리 테스트

## 참고사항

- Azure AI 자격증명은 사용자가 직접 입력해야 합니다
- appsettings.json은 .gitignore에 추가하는 것을 권장합니다
- 프로덕션 배포 시 환경변수 또는 Azure Key Vault 사용을 권장합니다
