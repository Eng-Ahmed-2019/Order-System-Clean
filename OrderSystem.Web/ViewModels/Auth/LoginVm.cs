using System.ComponentModel.DataAnnotations;

namespace OrderSystem.Web.ViewModels.Auth;

public class LoginVm
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}