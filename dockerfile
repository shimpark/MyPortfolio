# Dockerfile
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