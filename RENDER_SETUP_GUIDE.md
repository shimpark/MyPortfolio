# Render PostgreSQL 설정 가이드

## 🛠️ PostgreSQL 생성 설정 가이드

이미지의 각 항목을 아래와 같이 입력하시면 됩니다.

### 1. 기본 정보 입력

* **Name:** `myportfolio-db` (또는 프로젝트를 식별할 수 있는 고유한 이름)
* **Project:** (선택 사항) 기존에 생성한 프로젝트 폴더가 있다면 선택하고, 없다면 기본값인 `My project` 그대로 두셔도 무방합니다.
* **Database:** (선택 사항) 실제 DB 이름을 지정합니다. 기재하지 않으면 랜덤으로 생성되지만, 관리 편의를 위해 `todo_db` 처럼 입력하는 것을 추천합니다.
* **User:** (선택 사항) 접속 계정명입니다. 비워두면 랜덤 생성됩니다.

### 2. 환경 설정

* **Region:** **Singapore (Southeast Asia)** 를 그대로 유지하세요.
> **Tip:** 웹 서비스(Web Service)와 데이터베이스의 리전이 동일해야 네트워크 지연(Latency)이 최소화됩니다.

* **PostgreSQL Version:** **16** 또는 **17**을 추천합니다.
* 이미지에는 `18`이 선택되어 있으나, 현재 가장 안정적이고 호환성이 높은 버전은 15~17 사이입니다. ASP.NET Core 8의 Npgsql 라이브러리와 가장 무난하게 연동됩니다.

### 3. 인스턴스 타입 (화면 하단 스크롤 필요)

이미지 하단에는 보이지 않지만, 가장 중요한 단계입니다.

* **Instance Type:** 반드시 Free ($0/mo)를 선택해야 신용카드 결제 없이 생성이 완료됩니다.

---

## 🔗 생성 후 다음 단계 (ASP.NET Core 연결)

DB 생성이 완료되면 Render 대시보드에 **Internal Database URL** 또는 **External Database URL**이 나타납니다.

1. **Connection String 확보:** `postgres://user:password@hostname:port/dbname` 형태의 주소를 복사합니다.
2. **환경 변수 등록:**
* Render 웹 서비스 설정의 **Environment** 탭으로 이동합니다.
* `ConnectionStrings__DefaultConnection` 이라는 이름으로 위 주소를 등록합니다.


3. **Dapper 연동:**
* `Program.cs`에서 `NpgsqlConnection`을 사용할 때 이 환경 변수를 읽어오도록 설정되어 있는지 확인하세요.

---

## 🗄️ 테이블 생성

### SQL 초기화 스크립트 실행

생성된 PostgreSQL에서 아래 SQL을 실행합니다:

```sql
CREATE TABLE IF NOT EXISTS Todos (
    Id SERIAL PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    IsCompleted BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 테스트 데이터 (선택사항)
INSERT INTO Todos (Title, IsCompleted) VALUES
    ('ASP.NET Core 학습', false),
    ('Clean Architecture 이해', false),
    ('Dapper ORM 익히기', false),
    ('포트폴리오 배포', false);
```

### SQL 실행 방법

#### 방법 1: pgAdmin 또는 DBeaver 사용 (GUI)
1. 위 SQL을 복사
2. 쿼리 에디터에 붙여넣고 실행

#### 방법 2: psql CLI 사용
```bash
psql -U <username> -h <host> -d <database> -c "CREATE TABLE..."
```

#### 방법 3: SQL 파일로 실행
1. `init.sql` 파일에 위 SQL 저장
2. `psql -U <username> -h <host> -d <database> -f init.sql`

---

## ✅ ASP.NET Core 연결 확인

### Program.cs 확인사항

```csharp
// ✅ 이미 설정됨
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<IDbConnection>(sp => 
    new NpgsqlConnection(connectionString)
);
```

### appsettings.json 로컬 테스트 (선택사항)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=todo_db;Username=postgres;Password=your_password"
  }
}
```

---

## 🚀 배포 체크리스트

- [ ] PostgreSQL 데이터베이스 생성 (Free Tier)
- [ ] Connection String 복사
- [ ] Render 웹 서비스 Environment 변수 등록
- [ ] 테이블 및 초기 데이터 생성
- [ ] 로컬에서 연결 테스트 (선택사항)
- [ ] GitHub Push → GitHub Actions 자동 배포

---

## ⚠️ 주의사항

### 무료 티어 제약사항
- **30일 만료 후 데이터 자동 삭제** 
  - 중요 데이터는 SQL 스크립트로 백업 필요
- **500MB 스토리지 제한** (보통 포트폴리오용 충분)
- **인스턴스 자동 종료** (트래픽 없으면 24시간 후)

### 연결 문제 해결

**오류: "Unable to connect to host"**
- ✅ External Database URL 사용 (Internal 아님)
- ✅ Connection String 형식 확인
- ✅ 환경 변수 이름 정확히 입력

**오류: "no pg_hba.conf entry"**
- SSL 연결 필수: Connection String 뒤에 `?sslmode=require` 추가
- 예: `postgresql://...?sslmode=require`

---

## 📚 유용한 링크

- [Render PostgreSQL 공식 문서](https://render.com/docs/databases)
- [Npgsql Connection Strings](https://www.npgsql.org/doc/connection-string-parameters.html)
- [PostgreSQL 기본 명령어](https://www.postgresql.org/docs/current/app-psql.html)

