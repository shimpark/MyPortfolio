namespace MyPortfolio.Infrastructure;

public interface ISqlCacheService
{
    string GetQuery(string key);
}
