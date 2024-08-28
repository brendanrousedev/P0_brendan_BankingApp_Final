using System;
using System.Collections.Generic;

namespace P0_brendan_BankingApp_Final.POCO;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string CustomerUsername { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public byte[] Salt { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
