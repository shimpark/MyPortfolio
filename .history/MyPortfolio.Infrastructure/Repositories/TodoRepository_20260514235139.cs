using System.Data;
using Dapper;
using MyPortfolio.Application.DTOs;
using MyPortfolio.Application.Interfaces;

namespace MyPortfolio.Infrastructure.Repositories;

public class TodoRepository : BaseRepository, ITodoRepository
{
    public TodoRepository(IDbConnection db, ISqlCacheService sql) : base(db, sql) { }

    public async Task<IEnumerable<TodoDto>> GetTodosAllAsync()
    {
        var query = _sql.GetQuery("Todos_GetList");
        return await _db.QueryAsync<TodoDto>(query);
    }

    public async Task<int> InsertTodoAsync(TodoDto dto)
    {
        var query = _sql.GetQuery("Todos_Insert");
        return await _db.ExecuteAsync(query, dto);
    }
}
