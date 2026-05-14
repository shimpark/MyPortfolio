using System.Data;

namespace MyPortfolio.Infrastructure.Repositories;

public abstract class BaseRepository
{
    protected readonly IDbConnection _db;
    protected readonly ISqlCacheService _sql;

    protected BaseRepository(IDbConnection db, ISqlCacheService sql)
    {
        _db = db;
        _sql = sql;
    }
}
