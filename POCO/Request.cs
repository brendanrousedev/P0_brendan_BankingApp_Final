using System;
using System.Collections.Generic;

namespace P0_brendan_BankingApp_Final.POCO;

public partial class Request
{
    public int RequestId { get; set; }

    public int CustomerId { get; set; }

    public int? AccId { get; set; }

    public int AdminId { get; set; }

    public string RequestType { get; set; } = null!;

    public DateTime RequestDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Account? Acc { get; set; }

    public virtual Admin Admin { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
