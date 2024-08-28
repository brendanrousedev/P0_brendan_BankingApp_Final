using P0_brendan_BankingApp_Final.POCO;

public static class CustomerDao
{
    public static Customer GetCustomerByUsername(P0BrendanBankingDbContext Context, string username)
    {
        var customer = Context.Customers
            .Where(a => a.CustomerUsername == username)
            .FirstOrDefault();

        return customer;
    }
}