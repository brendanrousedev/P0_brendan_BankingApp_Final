using System.Diagnostics.CodeAnalysis;
using System.Security.AccessControl;
using Microsoft.SqlServer.Server;
using P0_brendan_BankingApp_Final.POCO;
using Spectre.Console;

public class CustomerController
{
    Customer customer;
    P0BrendanBankingDbContext Context;

    public CustomerController(P0BrendanBankingDbContext context, Customer customer)
    {
        Context = context;
        this.customer = customer;
    }

    public void Run()
    {
        IOConsole.PauseOutput();
    }
}
