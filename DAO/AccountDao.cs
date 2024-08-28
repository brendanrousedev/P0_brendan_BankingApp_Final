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
}