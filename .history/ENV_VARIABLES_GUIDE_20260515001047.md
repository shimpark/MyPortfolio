# Render 환경 변수 설정 예시

## 📝 Environment Variables (Render 웹 서비스)

Render 대시보드의 **Web Service → Environment** 탭에서 아래 변수들을 추가합니다.

### 1. PostgreSQL 연결 (필수)

```
Key: ConnectionStrings__DefaultConnection
Value: postgresql://username:password@host.render.com:5432/dbname?sslmode=require
```

> **형식 설명:**
> - `postgresql://` : Npgsql 드라이버
> - `username:password` : Render에서 생성한 사용자 계정
> - `host.render.com:5432` : Render PostgreSQL 호스트 및 포트
> - `dbname` : 데이터베이스 이름 (예: `todo_db`)
> - `?sslmode=require` : 필수! SSL 연결 강제

### 2. 환경 (선택사항)

```
Key: ASPNETCORE_ENVIRONMENT
Value: Production
```

### 3. 로그 레벨 (선택사항)

```
Key: Logging__LogLevel__Default
Value: Information
```

---

## 🔍 Connection String 확인하기

### Render 대시보드에서 확보

1. 좌측 메뉴 → **Databases**
2. 생성한 PostgreSQL 선택
3. **Connections** 섹션 확인

**다음 형식 중 하나 선택:**

| 방식 | URL 형식 | 사용처 |
|------|---------|--------|
| **External** | `postgresql://user:pass@host:5432/db?sslmode=require` | ASP.NET Core (권장) ✅ |
| **Internal** | `postgresql://user:pass@private-host:5432/db` | Render 내 다른 서비스 |

---

## 💻 로컬 개발 (appsettings.Development.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=todo_db;Username=postgres;Password=your_local_password;SslMode=Disable"
  }
}
```

> 로컬은 `SslMode=Disable` 사용 가능

---

## ✅ 배포 후 연결 테스트

### 방법 1: Render 대시보드 로그 확인
1. Web Service → **Logs** 탭
2. 애플리케이션 시작 로그 확인
3. DB 연결 에러가 없는지 확인

### 방법 2: API 호출로 테스트
```bash
curl https://your-app.onrender.com/api/todo
```

성공하면:
```json
[
  { "id": 1, "title": "...", "isCompleted": false },
  { "id": 2, "title": "...", "isCompleted": false }
]
```

---

## 🐛 문제 해결

### Connection Timeout
✅ Connection String에 `?sslmode=require` 포함되었나?
✅ PostgreSQL 인스턴스가 Running 상태인가?
✅ Render의 Instance가 Free Tier로 설정되었나?

### SSL 인증서 오류
✅ `sslmode=require` 또는 `sslmode=prefer` 사용
✅ 최신 Npgsql 버전 확인

### 환경 변수 미반영
✅ 환경 변수 수정 후 Web Service **Redeploy** 필요
✅ 수정 후 3~5분 대기 후 재시도

---

## 📚 참고 자료

- [Render PostgreSQL Docs](https://render.com/docs/databases)
- [Npgsql Connection Strings](https://www.npgsql.org/doc/connection-string-parameters.html)
- [ASP.NET Core Configuration](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration)
