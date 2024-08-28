using P0_brendan_BankingApp_Final.POCO;

public static class AccountDao
{
    public static Account GetAccountById(P0BrendanBankingDbContext Context, int id)
    {
        return Context.Accounts.Find(id);
    }

    public static void UpdateAccountBalance(P0BrendanBankingDbContext Context, Account account, decimal amount)
    {
        account.Balance = amount;
        Context.SaveChanges();
    }

    public static void UpdateAccountType(P0BrendanBankingDbContext Context, Account account, string type)
    {
        account.AccType = type;
        Context.SaveChanges();
    }

    public static List<Account> GetAllAccountsByUsername(P0BrendanBankingDbContext Context, Customer customer)
    {
        List<Account> customerAccount = new List<Account>();
        return Context.Accounts
                .Where(a => a.CustomerId == customer.CustomerId)
                .ToList();
    }

    public static void DeleteAccount(P0BrendanBankingDbContext Context, Account account)
    {
        Context.Remove(account);
        Context.SaveChanges();
    }

    public static void CreateAccount(P0BrendanBankingDbContext Context, Account account)
    {
        Context.Accounts.Add(account);
        Context.SaveChanges();
    }

    public static void WithdrawFunds(P0BrendanBankingDbContext context, Account account, decimal amount)
    {
        account.Balance -= amount;
        context.SaveChanges();
    }

    public static void DepositFunds(P0BrendanBankingDbContext context, Account account, decimal amount)
    {
        if (account.AccType == "Loan")
        {
            account.Balance -= amount;
        }
        else
        {
            account.Balance += amount;
        }

        context.SaveChanges();
    }
}