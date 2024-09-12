using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OrderSystem.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string OrderName { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public int NumberOfCopies { get; set; }

    public DateTime DateIn { get; set; }

    public bool IsDeleted { get; set; }

    public int UserId { get; set; }

    public int ClientId { get; set; }

    public int StatusId { get; set; } = 1;

    public int OrgId { get; set; }

    public string? PdfName { get; set; }
    public string? XLName { get; set; }



    public virtual ICollection<Serial> Serials { get; set; } = new List<Serial>();
    [AllowNull]
    public virtual Status Status { get; set; }

    [AllowNull]
    public virtual Client Client { get; set; }


    public virtual User User { get; set; } = null!;
}
