using System.ComponentModel.DataAnnotations;

namespace OrderSystem.Web.ViewModels.Auth;

public class RegisterVm
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string NationalId { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}