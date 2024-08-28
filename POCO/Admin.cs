using System;
using System.Collections.Generic;

namespace P0_brendan_BankingApp_Final.POCO;

public partial class Admin
{
    public int AdminId { get; set; }

    public string AdminUsername { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public byte[] Salt { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
