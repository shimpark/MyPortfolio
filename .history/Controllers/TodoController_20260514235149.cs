using Mediator;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Application.DTOs;
using MyPortfolio.Application.Handlers;

namespace MyPortfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : Controller
{
    private readonly IMediator _mediator;
    public TodoController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var result = await _mediator.Send(new GetTodosQuery());
        return Json(result);
    }

    // View 반환용 라우트
    [HttpGet("Index")]
    public IActionResult Index() => View();
}
