using System.ComponentModel.DataAnnotations;
using Entities;

namespace DTO;

public class UserDto : BaseDto<UserDto, User>
{
    [Required] [Display(Name = "نام")] public string FullName { get; set; } = null!;

    [Required]
    [Display(Name = "نام کاربری")]
    public string UserName { get; set; } = null!;
}

public class UserResDto : BaseDto<UserResDto, User>
{
    [Display(Name = "نام")] public string FullName { get; set; } = null!;

    [Display(Name = "نام کاربری")] public string UserName { get; set; } = null!;
}