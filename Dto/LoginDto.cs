using System.ComponentModel.DataAnnotations;

namespace DTO;

public class LoginDto
{
    [Display(Name = "نام کاربری")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string username { get; set; } = null!;
    [Display(Name = "رمزعبور")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [DataType(DataType.Password)]
    public string password { get; set; } = null!;

    public string grant_type { get; set; }

}