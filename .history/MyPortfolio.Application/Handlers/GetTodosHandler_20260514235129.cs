using Mediator;
using MyPortfolio.Application.DTOs;
using MyPortfolio.Application.Interfaces;

namespace MyPortfolio.Application.Handlers;

public record GetTodosQuery() : IRequest<IEnumerable<TodoDto>>;

public class GetTodosHandler : IRequestHandler<GetTodosQuery, IEnumerable<TodoDto>>
{
    private readonly ITodoRepository _repo;
    public GetTodosHandler(ITodoRepository repo) => _repo = repo;

    public async ValueTask<IEnumerable<TodoDto>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetTodosAllAsync();
    }
}
