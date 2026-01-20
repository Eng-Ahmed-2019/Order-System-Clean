using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrderSystem.Web.ViewModels.Admin;

public class ProductEditVm
{
    public int Id { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    [Display(Name = "SubCategory")]
    public int SubCategoryId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public List<SelectListItem> SubCategories { get; set; } = [];
}