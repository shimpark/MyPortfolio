using MyPortfolio.Application.DTOs;

namespace MyPortfolio.Application.Interfaces;

public interface ITodoRepository
{
    Task<IEnumerable<TodoDto>> GetTodosAllAsync();
    Task<int> InsertTodoAsync(TodoDto dto);
    Task<int> UpdateTodoAsync(int id, string title);
    Task<int> ToggleCompleteAsync(int id, bool isCompleted);
    Task<int> DeleteTodoAsync(int id);
}
