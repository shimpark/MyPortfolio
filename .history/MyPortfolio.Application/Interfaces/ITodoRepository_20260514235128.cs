using MyPortfolio.Application.DTOs;

namespace MyPortfolio.Application.Interfaces;

public interface ITodoRepository
{
    Task<IEnumerable<TodoDto>> GetTodosAllAsync();
    Task<int> InsertTodoAsync(TodoDto dto);
}
