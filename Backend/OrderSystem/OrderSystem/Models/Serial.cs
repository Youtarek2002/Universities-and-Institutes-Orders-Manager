using System;
using System.Collections.Generic;

namespace OrderSystem.Models;

public partial class Serial
{
    public int SerialId { get; set; }

    public string SerialNumber { get; set; } = null!;

    public int ClientId { get; set; }

    public int OrderId { get; set; }

    public DateTime DateIn { get; set; }

    public bool IsDeleted { get; set; }

    public int StatusId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
