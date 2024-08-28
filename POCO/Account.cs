using System;
using System.Collections.Generic;

namespace P0_brendan_BankingApp_Final.POCO;

public partial class Account
{
    public int AccId { get; set; }

    public int CustomerId { get; set; }

    public string AccType { get; set; } = null!;

    public decimal Balance { get; set; }

    public bool IsActive { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();
}
