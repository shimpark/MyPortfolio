namespace MyPortfolio.Infrastructure.Services;

public class SqlCacheService : ISqlCacheService
{
    private readonly Dictionary<string, string> _sqlCache = new();

    public SqlCacheService()
    {
        LoadSqlQueries();
    }

    private void LoadSqlQueries()
    {
        var basePath = Path.Combine(AppContext.BaseDirectory, "App_Data", "SqlQueries");

        if (!Directory.Exists(basePath))
        {
            throw new DirectoryNotFoundException($"SQL 폴더를 찾을 수 없습니다: {basePath}");
        }

        var sqlFiles = Directory.GetFiles(basePath, "*.sql");
        foreach (var file in sqlFiles)
        {
            var key = Path.GetFileNameWithoutExtension(file);
            var query = File.ReadAllText(file);
            _sqlCache[key] = query;
        }
    }

    public string GetQuery(string key)
    {
        if (_sqlCache.TryGetValue(key, out var query))
        {
            return query;
        }
        throw new KeyNotFoundException($"SQL 쿼리를 찾을 수 없습니다: {key}");
    }
}
