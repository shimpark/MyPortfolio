using MediatR;
using MyPortfolio.Application.Interfaces;

namespace MyPortfolio.Application.Handlers;

public record DeleteTodoCommand(int Id) : IRequest<bool>;

public class DeleteTodoHandler : IRequestHandler<DeleteTodoCommand, bool>
{
    private readonly ITodoRepository _repo;
    public DeleteTodoHandler(ITodoRepository repo) => _repo = repo;

    public async Task<bool> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var rowsAffected = await _repo.DeleteTodoAsync(request.Id);
        return rowsAffected > 0;
    }
}
