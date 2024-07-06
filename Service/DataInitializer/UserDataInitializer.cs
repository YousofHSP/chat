using Common.Utilities;
using Data.Contracts;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace Services.DataInitializer;

public class UserDataInitializer(IUserRepository userRepository) : IDataInitializer
{
    public void InitializerData()
    {
        if (!userRepository.TableNoTracking.Any(u => u.Mobile == "09140758738"))
        {
            var passwordHasher = new PasswordHasher<User>();
            
            var user = new User()
            {
                Mobile = "09140758738",
                Email = "admin@gmail.com",
                FullName = "yousof",
                Gender = GenderType.Male,
            };
            user.PasswordHash = passwordHasher.HashPassword(user, "1234");
            userRepository.Add(user);
        }
    }
}