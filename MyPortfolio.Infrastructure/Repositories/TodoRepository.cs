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
        // Returning ID from the Insert query
        return await _db.ExecuteScalarAsync<int>(query, dto);
    }

    public async Task<int> UpdateTodoAsync(int id, string title)
    {
        var query = _sql.GetQuery("Todos_Update");
        return await _db.ExecuteAsync(query, new { Id = id, Title = title });
    }

    public async Task<int> ToggleCompleteAsync(int id, bool isCompleted)
    {
        var query = _sql.GetQuery("Todos_ToggleComplete");
        return await _db.ExecuteAsync(query, new { Id = id, IsCompleted = isCompleted });
    }

    public async Task<int> DeleteTodoAsync(int id)
    {
        var query = _sql.GetQuery("Todos_Delete");
        return await _db.ExecuteAsync(query, new { Id = id });
    }
}
