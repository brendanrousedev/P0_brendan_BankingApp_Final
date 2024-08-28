using P0_brendan_BankingApp_Final.POCO;

public static class RequestDao
{
    public static int GetCheckbookRequestCount(P0BrendanBankingDbContext Context)
    {
        return Context.Requests.Where(r => r.Status == "Open").Count();
    }

    public static void ApproveAllCheckbookRequests(P0BrendanBankingDbContext Context)
    {
        foreach (var request in Context.Requests)
            {
                if (request.RequestType == "CHECKBOOK" && request.Status == "OPEN")
                {
                    request.Status = "APPROVED";

                }

            }
        
        Context.SaveChanges();
    }
}