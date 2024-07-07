using Common.Utilities;
using Data.Contracts;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace Services.DataInitializer;

public class UserDataInitializer(IUserRepository userRepository) : IDataInitializer
{
    public void InitializerData()
    {
        if (userRepository.TableNoTracking.Any(u => u.UserName == "admin")) return;
        var passwordHasher = new PasswordHasher<User>();
            
        var user = new User()
        {
            UserName = "admin",
            Email = "admin@gmail.com",
            FullName = "yousof",
            Gender = GenderType.Male,
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "1234");
        userRepository.Add(user);
    }
}