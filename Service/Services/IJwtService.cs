using Entities;
using Services;

namespace Service.Services
{
    public interface IJwtService
    {
        Task<AccessToken> GenerateAsync(User user);
    }
}