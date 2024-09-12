using System;
using System.Collections.Generic;

namespace OrderSystem.Models;

public partial class Organization
{
    public int OrgId { get; set; }

    public string OrgName { get; set; } = null!;

    public DateTime DateIn { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
