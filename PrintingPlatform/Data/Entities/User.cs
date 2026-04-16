using System.Collections.Generic;

namespace PrintingPlatform.Data.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public bool IsBlocked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}