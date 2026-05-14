using MediatR;
using MyPortfolio.Application.Interfaces;

namespace MyPortfolio.Application.Handlers;

public record UpdateTodoCommand(int Id, string Title) : IRequest<bool>;

public class UpdateTodoHandler : IRequestHandler<UpdateTodoCommand, bool>
{
    private readonly ITodoRepository _repo;
    public UpdateTodoHandler(ITodoRepository repo) => _repo = repo;

    public async Task<bool> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var rowsAffected = await _repo.UpdateTodoAsync(request.Id, request.Title);
        return rowsAffected > 0;
    }
}
