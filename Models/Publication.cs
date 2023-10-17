global using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class Publication
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime PostedDate { get; set; }
    [Required, DisplayName("Image")]
    [ValidateNever]
    public string ImgUrl { get; set; }
    [Required]
    public string UserId { get; set; }
    [ForeignKey("UserId"), ValidateNever]
    public ApplicationUser User { get; set; }
    public string? Description { get; set; }
}
