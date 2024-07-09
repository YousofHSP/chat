using Common.Utilities;
using Data.Contracts;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace Services.DataInitializer;

public class UserDataInitializer(IUserRepository userRepository) : IDataInitializer
{
    public void InitializerData()
    {
        if (userRepository.TableNoTracking.Any(u => u.UserName == "test")) return;
        var passwordHasher = new PasswordHasher<User>();
            
        var user = new User()
        {
            UserName = "test",
            Email = "test@gmail.com",
            FullName = "test",
            Gender = GenderType.Male,
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "1234");
        userRepository.Add(user);
    }
}