using System.ComponentModel.DataAnnotations;

namespace OrderSystem.Web.ViewModels.Admin;

public class SubCategoryEditVm
{
    public int Id { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}