using MyPortfolio.Application.DTOs;
using MyPortfolio.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace MyPortfolio.Application.Mappers;

[Mapper]
public partial class TodoMapper
{
    public partial TodoDto EntityToDto(Todo entity);
    public partial Todo DtoToEntity(TodoDto dto);
}
