# 🚀 MyPortfolio - Todo List Application

ASP.NET Core 8 기반으로 구축된 포트폴리오용 Todo List 웹 애플리케이션입니다. 클린 아키텍처(CQRS 패턴)를 적용하였으며, Render.com을 통한 무중단 CI/CD 배포 파이프라인이 구축되어 있습니다.

## 🛠 기술 스택

- **Backend:** ASP.NET Core 8 MVC, Web API
- **Architecture:** CQRS (MediatR), Repository Pattern
- **ORM / Database:** Dapper, PostgreSQL (Npgsql)
- **Frontend:** HTML5, CSS3 (Glassmorphism UI), Vue 3 (CDN), Axios
- **CI/CD & Hosting:** GitHub Actions, Docker, Render.com

---

## 📁 프로젝트 구조 (Clean Architecture)

```text
MyPortfolio/
├── .github/workflows/      # GitHub Actions 배포 파이프라인 (deploy.yml)
├── Controllers/            # 외부 HTTP 요청 처리 (TodoController, HomeController)
├── Views/                  # Vue 3를 이용해 렌더링된 반응형 UI
├── App_Data/SqlQueries/    # Dapper가 실행할 SQL 파일들을 물리적으로 분리
│
├── MyPortfolio.Application/# 비즈니스 로직 (MediatR Handlers, Interfaces, DTOs)
├── MyPortfolio.Domain/     # 핵심 도메인 모델 (Todo Entity)
└── MyPortfolio.Infrastructure/ # 외부 리소스 연동 (TodoRepository, SqlCacheService)
```

---

## 🌊 전체 배포 흐름

```text
[로컬 개발] → [GitHub Push] → [Actions: 빌드검증] → [Render Deploy Hook 호출] → [URL 접속]
```

---

## 💻 상세 가이드: 구축부터 배포까지 (Tutorial)

### 사전 준비 확인

```bash
dotnet --version   # 8.0.x 확인
git --version      # 설치 확인
```

---

### STEP 1. 로컬 환경 테스트 및 설정

이미 생성된 본 프로젝트를 로컬에서 테스트하려면 다음 과정을 거칩니다.

**1. 로컬 설정 분리 (`appsettings.Development.json`)**
운영 환경에 비밀번호가 노출되는 것을 방지하기 위해 로컬 개발용 연결 문자열은 `appsettings.Development.json`에 작성합니다.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=todo_db;Username=postgres;Password=로컬비밀번호;SslMode=Prefer;"
  }
}
```

**2. 데이터베이스 초기화 (PostgreSQL)**

```sql
CREATE TABLE IF NOT EXISTS Todos (
    Id SERIAL PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    IsCompleted BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

**3. 로컬 실행 확인**

```bash
dotnet run
# → http://localhost:8080/Todo/Index 접속 확인 후 Ctrl+C 종료
```

> **참고:** `Program.cs` 에는 Render 호환성을 위해 PORT 환경변수를 동적으로 읽는 로직이 구현되어 있습니다.

```csharp
// ✅ Render가 주입하는 PORT 환경변수 읽기 (없으면 8080)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
```

---

### STEP 2. Dockerfile 작성 (프로젝트 루트)

Render 배포를 위한 `Dockerfile` 구성입니다.

```dockerfile
# ── 1단계: 빌드 ──────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# csproj만 먼저 복사 → restore (Docker 레이어 캐시 최적화)
COPY *.csproj ./
RUN dotnet restore

# 나머지 소스 복사 후 publish
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# ── 2단계: 런타임 (SDK 제거, 이미지 경량화) ──
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render는 PORT 환경변수를 동적으로 주입 (기본값 10000)
ENV PORT=10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "MyPortfolio.dll"]
```

`.dockerignore` 생성 (빌드 속도 향상):

```text
bin/
obj/
.git/
.vs/
*.user
```

---

### STEP 3. .gitignore & GitHub Push

보안 및 불필요한 파일 제외를 위한 `.gitignore` 설정 후 GitHub에 푸시합니다.

```text
bin/
obj/
.vs/
*.user
appsettings.Development.json
```

```bash
git init
git add .
git commit -m "init: ASP.NET Core 8 + Dockerfile"

git remote add origin https://github.com/{본인ID}/MyPortfolio.git
git branch -M main
git push -u origin main
```

---

### STEP 4. Render.com 데이터베이스 및 Web Service 생성

**① 데이터베이스 생성 (PostgreSQL)**
Render 대시보드에서 PostgreSQL 데이터베이스를 무료 티어로 생성합니다.

**② New → Web Service**
`Dashboard` → `[+ New]` → `[Web Service]`

**③ GitHub 리포지토리 연결**
`Connect a repository` → GitHub 연동 → MyPortfolio 리포지토리 선택

**④ 서비스 설정**

```text
Name:        myportfolio          ← URL에 사용됨
Region:      Singapore            ← 한국과 가장 가까운 무료 리전
Branch:      main
Runtime:     Docker               ← 자동 감지됨
Instance 주소: Free               ← 반드시 Free 선택

[Create Web Service] 클릭
```

**⑤ 환경 변수 (Environment Variables) 등록 🌟매우 중요🌟**
운영 서버는 `appsettings.json`을 비워두고 Render 대시보드에서 환경 변수를 주입합니다.

- **Key:** `ConnectionStrings__DefaultConnection` (언더바 2개 유의)
- **Value:** `Host=dpg-...-a;Database=...;Username=...;Password=...;` (DB 생성 시 부여받은 **Internal Database URL**을 Npgsql Key-Value 형태로 변환한 값)

**⑥ 첫 번째 자동 배포 진행 확인**
Render 대시보드 로그에서 `Your service is live` 메시지를 확인합니다.

---

### STEP 5. Render Auto-Deploy 비활성화 + Deploy Hook URL 복사

> GitHub Actions에서 빌드 성공 후에만 배포하기 위해 자동배포를 끄고 Hook으로 제어합니다.

```text
Render 대시보드
→ 해당 서비스 클릭
→ [Settings] 탭
→ Auto-Deploy: [No] 로 변경 → Save

→ 같은 Settings 페이지에서
→ Deploy Hook 섹션
→ URL 복사 (https://api.render.com/deploy/srv-xxx...?key=yyy 형태)
```

---

### STEP 6. GitHub Secrets 등록

Deploy Hook URL은 비밀 값이므로 GitHub 리포지토리 Secrets에 저장해야 합니다.

```text
GitHub 리포지토리
→ Settings
→ Secrets and variables → Actions
→ [New repository secret]

이름: RENDER_DEPLOY_HOOK_URL
값:   (STEP 5에서 복사한 Deploy Hook URL 전체)

→ [Add secret]
```

---

### STEP 7. GitHub Actions Workflow 작성

`.github/workflows/deploy.yml` 파이프라인 구성 파일입니다.

```yaml
name: CI/CD → Render 배포

on:
  push:
    branches: ["main"] # main 브랜치 push 시 실행
  workflow_dispatch: # GitHub UI에서 수동 실행 가능

jobs:
  build-and-deploy:
    name: 🔨 Build & 🚀 Deploy
    runs-on: ubuntu-latest # GitHub 무료 VM

    steps:
      # ① 소스코드 체크아웃
      - name: 📥 Checkout
        uses: actions/checkout@v4

      # ② .NET 8 SDK 설치
      - name: ⚙️ Setup .NET 8
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "8.0.x"

      # ③ NuGet 패키지 복원
      - name: 📦 Restore
        run: dotnet restore

      # ④ Release 빌드 (실패 시 이후 단계 실행 안 됨)
      - name: 🔨 Build
        run: dotnet build --configuration Release --no-restore

      # ⑤ Render Deploy Hook 호출 (빌드 성공 시에만 실행)
      - name: 🚀 Trigger Render Deploy
        run: |
          curl -s -o /dev/null -w "HTTP Status: %{http_code}\n" \
            "${{ secrets.RENDER_DEPLOY_HOOK_URL }}"
```

---

### STEP 8. Push → Actions 실행 → 배포 확인

```bash
git add .
git commit -m "ci: GitHub Actions + Render 배포 파이프라인 구성"
git push origin main
```

**GitHub Actions 탭에서 실시간 확인:**

```text
▶ CI/CD → Render 배포   running...
  └── Build & Deploy
        ✅ Checkout        (2s)
        ✅ Setup .NET 8    (15s)
        ✅ Restore         (10s)
        ✅ Build           (20s)
        ✅ Trigger Render  HTTP Status: 201  ← 성공
```

**Render 대시보드에서 배포 로그 확인:**

```text
Render → 서비스 → [Events] 탭
→ "Deploy started" → "Deploy live" 메시지 확인 (약 2~3분)
```

---

### STEP 9. URL 접속 확인

```text
https://myportfolio-55dv.onrender.com/

✅ GitHub Actions + Render 배포 성공!
🕐 배포 시각: 2026-05-14 12:34:56 UTC
ASP.NET Core 8 · GitHub Actions · Render
```

---

> [!NOTE]
> 자세한 Render PostgreSQL 생성 및 팁은 [`RENDER_SETUP_GUIDE.md`](./RENDER_SETUP_GUIDE.md) 문서를 참고해 주세요. 막히는 단계가 생기면 로그를 확인하여 트러블슈팅을 진행할 수 있습니다.
