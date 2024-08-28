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
        MENU_NAME = "***************************\n" +
                    $"Admin Menu - {admin.AdminUsername}\n" +
                    "***************************";


        var menu = new SelectionPrompt<string>()
            .Title("Use the arrow keys to select an operation")
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
            AnsiConsole.Clear();
            IOConsole.WriteMenu(MENU_NAME);
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

            IOConsole.PauseOutput();
        }
    }

    private void ApproveAllCheckbookRequests()
    {
        MENU_NAME = "***************************\n" +
                    $"Approve Checkbook Requests\n" +
                    "***************************";
        AnsiConsole.Clear();
        IOConsole.WriteMenu(MENU_NAME);
        int requestCount = RequestDao.GetCheckbookRequestCount(Context);
        if (requestCount == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no open requests.[/]");
            
        }
        else if (AnsiConsole.Confirm($"There are {requestCount} request(s) open, approve all?"))
        {
            RequestDao.ApproveAllCheckbookRequests(Context);
            AnsiConsole.Markup($"[blue]All {requestCount} request(s) have been approved! Press any key to continue...[/]");
            
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]No checkbook requests were approved.[/]");
            IOConsole.PauseOutput();
        }


    }

    private void ResetCustomerPassword()
    {

        MENU_NAME = "***********************\n"
                  + "Reset Customer Password\n"
                  + "***********************";
        AnsiConsole.Clear();
        IOConsole.WriteMenu(MENU_NAME);

        var username = IOConsole.GetUsername("Customer");
        Customer? customer = CustomerDao.GetCustomerByUsername(Context, username);
        if (customer == null)
        {
            AnsiConsole.MarkupLine($"[red]{customer} could not be found.");
        }

        else if (AnsiConsole.Confirm($"Reset password for {username}?"))
        {
            CustomerDao.ResetPassword(Context, customer);
            AnsiConsole.WriteLine($"\nThe password for {username} was reset to the default!");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]No password was reset for any customer.[/]");
        }
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
