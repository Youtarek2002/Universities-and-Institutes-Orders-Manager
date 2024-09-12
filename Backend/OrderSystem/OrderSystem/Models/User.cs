using System;
using System.Collections.Generic;

namespace OrderSystem.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string UserFirstName { get; set; } = null!;

    public string UserLastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime DateIn { get; set; }

    public bool IsDeleted { get; set; }

    public int RoleId { get; set; }

    public int? OrgId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Organization? Org { get; set; }

    public virtual Role Role { get; set; } = null!;
}
