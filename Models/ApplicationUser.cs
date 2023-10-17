using Microsoft.AspNetCore.Identity;

namespace Models;

public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Subscribed From
    /// </summary>
    [ValidateNever]
    public ICollection<Subscription> Followers { get; set; } = new List<Subscription>();
    /// <summary>
    /// Subscribed to
    /// </summary>
    [ValidateNever]
    public ICollection<Subscription> Following { get; set; } = new List<Subscription>();
}
