using System.ComponentModel.DataAnnotations;

namespace OrderSystem.Web.ViewModels.Admin;

public class ProductEditVm
{
    public int Id { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int SubCategoryId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}