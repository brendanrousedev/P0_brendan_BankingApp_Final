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

    public static void ResetPassword(P0BrendanBankingDbContext Context, Customer customer)
    {
            string defaultPassword = "password1";
            byte[] salt = PasswordUtils.GenerateSalt();
            customer.PasswordHash = PasswordUtils.HashPassword(defaultPassword, salt);
            customer.Salt = salt;
            Context.SaveChanges();
    }

    public static void CreateCustomer(P0BrendanBankingDbContext Context, Customer customer)
    {
        Context.Customers.Add(customer);
        Context.SaveChanges();
    }
}