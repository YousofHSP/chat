using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using Data.Contracts;
using DTO;
using Entities.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFramework.Filters;

namespace WebFramework.Api;

[ApiController]
[Authorize]
[ApiVersion("1")]
[Route("api/v{version:ApiVersion}/admin/[controller]")]

public class CrudController<TDto, TResDto, TEntity, TKey>(IRepository<TEntity> repository, IMapper mapper)
    : BaseController
    where TEntity : class, IEntity<TKey>
    where TDto : BaseDto<TDto, TEntity, TKey>
    where TResDto : BaseDto<TResDto, TEntity, TKey>
{
    protected readonly IRepository<TEntity> Repository = repository;
    protected readonly IMapper Mapper = mapper;

    [HttpGet]
    public virtual async Task<ApiResult<List<TResDto>>> Get(CancellationToken cancellationToken)
    {
        var list = await Repository.TableNoTracking
            .ProjectTo<TResDto>(Mapper.ConfigurationProvider).ToListAsync(cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id}")]
    public virtual async Task<ApiResult<TResDto>> Get(TKey id, CancellationToken cancellationToken)
    {
        var dto = await Repository.TableNoTracking.ProjectTo<TResDto>(Mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id!.Equals(id), cancellationToken);
        if (dto == null)
            return NotFound();
        return dto;
    }

    [HttpPost]
    public virtual async Task<ApiResult<TResDto>> Create(TDto dto, CancellationToken cancellationToken)
    {
        var model = dto.ToEntity(Mapper);

        await Repository.AddAsync(model, cancellationToken);
        var resultDto = await Repository.TableNoTracking.ProjectTo<TResDto>(Mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id!.Equals(model.Id), cancellationToken);
        return resultDto!;
    }

    [HttpPatch("{id}")]
    public virtual async Task<ApiResult<TResDto>> Update(TKey id, TDto dto, CancellationToken cancellationToken)
    {
        var model = await Repository.GetByIdAsync(cancellationToken, id);

        model = dto.ToEntity(model, Mapper);

        await Repository.UpdateAsync(model, cancellationToken);
        var resultDto = await Repository.TableNoTracking
            .ProjectTo<TResDto>(Mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id!.Equals(model.Id), cancellationToken);
        return resultDto!;
    }

    [HttpDelete("{id}")]
    public virtual async Task<ApiResult> Delete(TKey id, CancellationToken cancellationToken)
    {
        var model = await Repository.GetByIdAsync(cancellationToken, id!);
        if (model is null) throw new NotFoundException();
        await Repository.DeleteAsync(model, cancellationToken);

        return Ok();
    }

}
public class CrudController<TDto, TResDto, TEntity>(IRepository<TEntity> repository, IMapper mapper)
    : CrudController<TDto, TResDto, TEntity, int>(repository, mapper)
    where TEntity : class, IEntity<int>
    where TDto : BaseDto<TDto, TEntity, int>
    where TResDto : BaseDto<TResDto, TEntity, int>;

public class CrudController<TDto, TEntity>(IRepository<TEntity> repository, IMapper mapper)
    : CrudController<TDto, TDto, TEntity, int>(repository, mapper)
    where TEntity : class, IEntity<int>
    where TDto : BaseDto<TDto, TEntity, int>;
