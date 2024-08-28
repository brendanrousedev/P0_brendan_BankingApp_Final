using System.Diagnostics.CodeAnalysis;
using System.Security.AccessControl;
using Microsoft.SqlServer.Server;
using P0_brendan_BankingApp_Final.POCO;
using Spectre.Console;

public class AdminController
{
    Admin admin;
    P0BrendanBankingDbContext Context;
    string MENU_NAME = "";

    public AdminController(P0BrendanBankingDbContext context, Admin admin)
    {
        Context = context;
        this.admin = admin;
        MENU_NAME = "********************************\n" +
                    $"Admin Menu - {admin.AdminUsername}\n" +
                    "**********************************";
    }

    public void Run()
    {
        const string CREATE = "Create a New Account",
        DELETE = "Delete Account",
        UPDATE = "Update Account Details",
        SUMMARY = "Display Summary",
        RESET = "Reset Customer Password",
        APPROVE = "Approve Checkbook Request",
        EXIT = "Exit to Main Menu";


        var menu = new SelectionPrompt<string>()
            .Title("***************************"
                  + $"\nAdmin menu - {admin.AdminUsername}"
                  + "\n***************************")
            .PageSize(10)
            .HighlightStyle(new Style(foreground: Color.Green, background: Color.Black))
            .AddChoices(new[] {
                CREATE,
                DELETE,
                UPDATE,
                SUMMARY,
                RESET,
                APPROVE,
                EXIT
            });

        bool isRunning = true;
        while (isRunning)
        {
            var choice = AnsiConsole.Prompt(menu);
            switch (choice)
            {
                case CREATE:
                    CreateAccount();
                    break;
                case DELETE:
                    DeleteAccount();
                    break;
                case UPDATE:
                    UpdateAccount();
                    break;
                case SUMMARY:
                    GetSummary();
                    break;
                case RESET:
                    ResetCustomerPassword();
                    break;
                case APPROVE:
                    ApproveAllCheckbookRequests();
                    break;
                case EXIT:
                    if (AnsiConsole.Confirm("Return to the main menu?"))
                    {
                        isRunning = false;
                    }
                    break;
            }
            Console.ReadKey();
        }
    }

    private void ApproveAllCheckbookRequests()
    {
        throw new NotImplementedException();
    }

    private void ResetCustomerPassword()
    {
        throw new NotImplementedException();
    }

    private void GetSummary()
    {
        throw new NotImplementedException();
    }

    private void UpdateAccount()
    {
        throw new NotImplementedException();
    }

    private void DeleteAccount()
    {
        throw new NotImplementedException();
    }

    private void CreateAccount()
    {
        throw new NotImplementedException();
    }
}
