global using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class Subscription
{
    [Key]
    public string ToId { get; set; }
    [Key]
    public string FromId { get; set; }
    public virtual ApplicationUser SubscribedTo { get; set; }
    public virtual ApplicationUser SubscribedFrom { get; set; }
}
