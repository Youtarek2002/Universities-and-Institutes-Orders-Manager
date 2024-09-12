using System;
using System.Collections.Generic;

namespace OrderSystem.Models;

public partial class Status
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public DateTime DateIn { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Serial> Serials { get; set; } = new List<Serial>();
}
