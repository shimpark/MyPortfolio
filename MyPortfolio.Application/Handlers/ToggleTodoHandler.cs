using MediatR;
using MyPortfolio.Application.Interfaces;

namespace MyPortfolio.Application.Handlers;

public record ToggleTodoCommand(int Id, bool IsCompleted) : IRequest<bool>;

public class ToggleTodoHandler : IRequestHandler<ToggleTodoCommand, bool>
{
    private readonly ITodoRepository _repo;
    public ToggleTodoHandler(ITodoRepository repo) => _repo = repo;

    public async Task<bool> Handle(ToggleTodoCommand request, CancellationToken cancellationToken)
    {
        var rowsAffected = await _repo.ToggleCompleteAsync(request.Id, request.IsCompleted);
        return rowsAffected > 0;
    }
}
