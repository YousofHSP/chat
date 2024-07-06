using Api.Models;
using AutoMapper;
using Common.Exceptions;
using Data.Contracts;
using DTO;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using WebFramework.Api;

namespace Api.Controller.v1;

[ApiVersion("1")]
public class UserController(IUserRepository repository, IMapper mapper, UserManager<User> userManager, IJwtService jwtService) : CrudController<UserDto, UserResDto, User>(repository, mapper)
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
    
}