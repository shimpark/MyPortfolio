# Render PostgreSQL 설정 가이드

## 📋 PostgreSQL 데이터베이스 생성

### 1단계: Render 대시보드에서 PostgreSQL 생성

#### 기본 정보 입력
- **Name:** `myportfolio-db` (또는 프로젝트명)
- **Database:** `todo_db` (선택사항 - 실제 DB 이름)
- **User:** (비워두면 자동 생성)
- **Region:** **Singapore (Southeast Asia)** ⭐ **중요**

#### 환경 설정
- **PostgreSQL Version:** `16` 또는 `17` (권장)
  - v18은 최신이지만 ASP.NET Core 8과의 호환성을 위해 15~17 추천
- **Instance Type:** **Free ($0/mo)** ⭐ **필수**

---

## 🔗 Connection String 설정

### 1단계: Connection String 확보

DB 생성 완료 후, Render 대시보드의 **Database Details**에서:
- **External Database URL** 복사 (Npgsql에서 사용)
- 형식: `postgresql://user:password@host:5432/dbname`

### 2단계: 환경 변수 등록

**Render 웹 서비스 설정 → Environment 탭:**

```
ConnectionStrings__DefaultConnection=postgresql://user:password@host:5432/dbname
```

> ⚠️ `ConnectionStrings__DefaultConnection` 형식 주의!
> 두 개의 언더스코어(`__`)로 `:` 를 대체합니다.

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

