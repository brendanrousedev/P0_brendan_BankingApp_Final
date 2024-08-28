using P0_brendan_BankingApp_Final.POCO;

public static class AccountDao
{
    public static Account GetAccountById(P0BrendanBankingDbContext Context, int id)
    {
        return Context.Accounts.Find(id);
    }
}