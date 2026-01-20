using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrderSystem.Web.ViewModels.Admin;

public class SubCategoryEditVm
{
    public int Id { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<SelectListItem> Categories { get; set; } = [];
}