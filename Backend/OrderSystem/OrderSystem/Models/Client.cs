using System;
using System.Collections.Generic;

namespace OrderSystem.Models;

public partial class Client
{
    public int ClientId { get; set; }

    public string ClientName { get; set; } = null!;

    public string FixedPart { get; set; } = null!;

    public int OrgId { get; set; }

    public DateTime DateIn { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Organization Org { get; set; } = null!;
}
