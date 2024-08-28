using P0_brendan_BankingApp_Final.POCO;

public static class TransactionLogDao
{
    public static void CreateTransactionLog(P0BrendanBankingDbContext Context, Account account, decimal amount, string type)
    {
        TransactionLog tl = new TransactionLog
            {
                AccId = account.AccId,
                Acc = account,
                Amount = amount,
                TransactionDate = DateTime.Now,
                TransactionType = "Adjustment"
            };
            Context.SaveChanges();
    }
}