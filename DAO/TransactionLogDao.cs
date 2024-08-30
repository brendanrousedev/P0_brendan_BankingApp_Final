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
                TransactionType = type
            };
        Context.TransactionLogs.Add(tl);
        Context.SaveChanges();
    }

    public static void DeleteAllTransactionsForAccount(P0BrendanBankingDbContext Context, Account account)
    {
        foreach (var tl in Context.TransactionLogs)
        {
            if (tl.AccId == account.AccId)
            {
                Context.Remove(tl);
            }
        }

        Context.SaveChanges();
    }

    public static List<TransactionLog> GetTransactionLogs(P0BrendanBankingDbContext context, Customer customer, int limit)
    {
        return context.TransactionLogs
            .Where(t => t.Acc.CustomerId == customer.CustomerId)
            .OrderByDescending(t => t.TransactionDate)
            .Take(limit)
            .ToList();
    }

    
}