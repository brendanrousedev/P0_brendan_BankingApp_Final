using P0_brendan_BankingApp_Final.POCO;

public static class AdminDao
{
    public static Admin GetAdminByUsername(P0BrendanBankingDbContext Context, string username)
    {
        var admin = Context.Admins
            .Where(a => a.AdminUsername == username)
            .FirstOrDefault();

        return admin;
    }
}