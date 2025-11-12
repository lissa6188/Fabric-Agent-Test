# 설정 가이드

## Azure OpenAI 설정

### 1. Azure Portal에서 Azure OpenAI 리소스 생성

1. [Azure Portal](https://portal.azure.com)에 로그인
2. "리소스 만들기" > "Azure OpenAI" 검색
3. Azure OpenAI 리소스 생성
   - 구독 선택
   - 리소스 그룹 선택/생성
   - 지역 선택 (예: Korea Central, East US)
   - 이름 지정
   - 가격 책정 계층 선택

### 2. 모델 배포

1. Azure OpenAI Studio로 이동
2. "배포" 섹션에서 "새 배포 만들기"
3. 모델 선택 (예: gpt-4o, gpt-4, gpt-35-turbo)
4. 배포 이름 지정 (예: gpt-4o)
5. 배포 완료 대기

### 3. 엔드포인트 및 키 확인

1. Azure Portal의 Azure OpenAI 리소스로 이동
2. "키 및 엔드포인트" 메뉴 클릭
3. 다음 정보 복사:
   - **엔드포인트**: `https://<your-resource-name>.openai.azure.com/`
   - **키**: KEY 1 또는 KEY 2

### 4. appsettings.json 설정

`src/FabricAgent.Backend/appsettings.json` 파일을 열고 다음 값을 수정:

```json
{
  "AzureAI": {
    "ProjectEndpoint": "https://<your-resource-name>.openai.azure.com/",
    "DeploymentName": "gpt-4o",
    "ApiKey": "<your-api-key>"
  }
}
```

**중요**: 
- `ProjectEndpoint`는 반드시 `/`로 끝나야 합니다
- `DeploymentName`은 Azure OpenAI Studio에서 생성한 배포 이름과 정확히 일치해야 합니다
- `ApiKey`는 절대 GitHub 등 공개 저장소에 커밋하지 마세요

### 5. 보안 설정 (선택사항)

개발 환경에서는 User Secrets를 사용하여 민감 정보를 안전하게 관리할 수 있습니다:

```bash
cd src/FabricAgent.Backend
dotnet user-secrets init
dotnet user-secrets set "AzureAI:ApiKey" "<your-api-key>"
dotnet user-secrets set "AzureAI:ProjectEndpoint" "https://<your-resource-name>.openai.azure.com/"
```

프로덕션 환경에서는:
- Azure Key Vault 사용
- Managed Identity 사용
- 환경 변수 설정

## Microsoft Fabric 설정 (선택사항)

향후 Fabric 데이터 연동을 위한 설정:

### 1. Fabric Workspace 생성

1. [Microsoft Fabric](https://app.fabric.microsoft.com)에 로그인
2. 새 Workspace 생성
3. Lakehouse 또는 Data Warehouse 생성

### 2. 연결 문자열 확인

1. Fabric Workspace에서 설정 확인
2. 연결 정보 복사

### 3. appsettings.json 업데이트

```json
{
  "Fabric": {
    "ConnectionString": "<your-fabric-connection-string>",
    "WorkspaceId": "<your-workspace-id>",
    "LakehouseName": "<your-lakehouse-name>"
  }
}
```

## 환경별 설정

### 개발 환경

`appsettings.Development.json` 파일 생성:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Azure.AI.OpenAI": "Debug"
    }
  },
  "AzureAI": {
    "ProjectEndpoint": "https://<dev-resource>.openai.azure.com/",
    "DeploymentName": "gpt-4o",
    "ApiKey": "<dev-api-key>"
  }
}
```

### 프로덕션 환경

환경 변수 사용:

```bash
export AzureAI__ProjectEndpoint="https://<prod-resource>.openai.azure.com/"
export AzureAI__ApiKey="<prod-api-key>"
export AzureAI__DeploymentName="gpt-4o"
```

## 문제 해결

### "Azure AI credentials not configured" 오류

- `appsettings.json`에 `AzureAI:ProjectEndpoint`와 `AzureAI:ApiKey`가 설정되어 있는지 확인
- 값이 비어있지 않은지 확인

### "Deployment not found" 오류

- `DeploymentName`이 Azure OpenAI Studio의 배포 이름과 정확히 일치하는지 확인
- 배포가 활성 상태인지 확인

### 할당량 초과 오류

- Azure Portal에서 Azure OpenAI 리소스의 할당량 확인
- 필요시 할당량 증가 요청

### 인증 오류

- API Key가 유효한지 확인
- Azure OpenAI 리소스가 활성 상태인지 확인
- 네트워크 방화벽 설정 확인

## 다음 단계

설정이 완료되면 [README.md](../README.md)의 "실행 방법" 섹션을 참고하여 애플리케이션을 실행하세요.
