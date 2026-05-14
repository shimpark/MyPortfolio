using MediatR;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTodoCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Title)) return BadRequest("Title is required.");
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        var success = await _mediator.Send(command);
        return success ? Ok() : NotFound();
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> ToggleComplete(int id, [FromBody] ToggleTodoCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        var success = await _mediator.Send(command);
        return success ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _mediator.Send(new DeleteTodoCommand(id));
        return success ? Ok() : NotFound();
    }

    // View 반환용 라우트
    [HttpGet("/Todo/Index")]
    public IActionResult Index() => View();
}
