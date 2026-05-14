using MediatR;
using MyPortfolio.Application.DTOs;
using MyPortfolio.Application.Interfaces;

namespace MyPortfolio.Application.Handlers;

public record CreateTodoCommand(string Title) : IRequest<TodoDto>;

public class CreateTodoHandler : IRequestHandler<CreateTodoCommand, TodoDto>
{
    private readonly ITodoRepository _repo;
    public CreateTodoHandler(ITodoRepository repo) => _repo = repo;

    public async Task<TodoDto> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        // Get Korean Standard Time
        var kstZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
        var nowKst = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, kstZone);

        var newTodo = new TodoDto
        {
            Title = request.Title,
            IsCompleted = false,
            CreatedAt = nowKst
        };

        var id = await _repo.InsertTodoAsync(newTodo);
        newTodo.Id = id;
        
        return newTodo;
    }
}
