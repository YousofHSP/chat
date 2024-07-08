using Api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using Common.Utilities;
using Data.Contracts;
using DTO;
using Entities;
using Entities.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Services;
using WebFramework.Api;

namespace Api.Controller.v1;

[ApiVersion("1")]
public class UserController(IUserRepository repository, IRepository<Follow> followRepository, IMapper mapper, UserManager<User> userManager, IJwtService jwtService) : CrudController<UserDto, UserResDto, User>(repository, mapper)
{
    
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult> Token([FromForm]TokenRequest tokenRequest, CancellationToken cancellationToken)
    {
        var user = await repository.GetByMobileAndPass(tokenRequest.username, tokenRequest.password, cancellationToken);
        if (user is null)
            throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");
        await userManager.UpdateSecurityStampAsync(user);
        var jwt = await jwtService.GenerateAsync(user);
        return new JsonResult(jwt);

    }

    public async Task<ActionResult> GetByFilter(string filter, CancellationToken cancellationToken)
    {
        var users = await repository.TableNoTracking
            .Where(i => i.UserName!.Contains(filter) || i.FullName.Contains(filter))
            .ProjectTo<UserResDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return Ok(users);

    }

    public async Task<ActionResult> GetFollowings(CancellationToken cancellationToken)
    {
        var userId = int.Parse( User.Identity!.GetUserId());
        var users = await repository.GetFollowings(userId, cancellationToken);
        var dtos = mapper.Map<List<UserDto>>(users);
        return Ok(dtos);
    }

    public async Task<ActionResult> Follow(int userId, CancellationToken cancellationToken)
    {
        var followerId = User.Identity!.GetUserId<int>();
        var model = new Follow
        {
            UserId = userId,
            FollowerId = followerId
        };
        await followRepository.AddAsync(model, cancellationToken);
        return Ok();
    }
    
}