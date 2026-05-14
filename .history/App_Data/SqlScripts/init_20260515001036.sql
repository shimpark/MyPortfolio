-- MyPortfolio 데이터베이스 초기화 스크립트
-- Render PostgreSQL 또는 로컬 PostgreSQL에서 실행

-- ✅ Todos 테이블 생성
CREATE TABLE IF NOT EXISTS Todos (
    Id SERIAL PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    IsCompleted BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ✅ 테스트 데이터 삽입 (선택사항)
-- 초기 데이터가 필요 없으면 아래 주석 제거 후 삭제하세요
INSERT INTO Todos (Title, IsCompleted) VALUES
    ('ASP.NET Core Clean Architecture 학습', false),
    ('Dapper ORM으로 데이터베이스 연동', false),
    ('MediatR를 이용한 CQRS 패턴 구현', false),
    ('Vue3 CDN으로 프론트엔드 구성', false),
    ('Render에 배포 및 PostgreSQL 연동', false);

-- ✅ 인덱스 생성 (조회 성능 최적화)
CREATE INDEX IF NOT EXISTS idx_todos_is_completed ON Todos(IsCompleted);
CREATE INDEX IF NOT EXISTS idx_todos_created_at ON Todos(CreatedAt DESC);

-- ✅ 테이블 확인 (실행 후 조회)
-- SELECT * FROM Todos;
-- SELECT COUNT(*) FROM Todos;
