using P0_brendan_BankingApp_Final.POCO;
using Spectre.Console;

public class LoginController
{
    P0BrendanBankingDbContext Context;
    Admin admin;
    Customer customer;
    const string MENU_NAME = "**********\n" +
                             "Main Login\n" +
                             "**********\n";

    public LoginController(P0BrendanBankingDbContext context)
    {
        Context = context;
    }

    internal void Run()
    {
        AnsiConsole.Clear();

        const string ADMIN = "Administrator", CUSTOMER = "Customer", EXIT = "Exit";
        var menu = new SelectionPrompt<string>()
            .Title("What type of user are you?")
            .PageSize(10)
            .HighlightStyle(new Style(foreground: Color.Green, background: Color.Black))
            .AddChoices(new[] {
                ADMIN,
                CUSTOMER,
                EXIT
            });

        bool isRunning = true;
        while (isRunning)
        {
            AnsiConsole.Clear();
            IOConsole.WriteMenu(MENU_NAME);
            var choice = AnsiConsole.Prompt(menu);
            string username = "", password = "";
            switch (choice)
            {
                case ADMIN:
                    username = IOConsole.GetUsername(ADMIN);
                    password = IOConsole.GetPassword(ADMIN);
                    if (this.VerifyAdmin(username, password))
                    {
                        IOConsole.WriteLoginSuccess(username, ADMIN);
                        AdminController ac = new AdminController(Context, AdminDao.GetAdminByUsername(Context, username));
                        ac.Run();
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Please check your password and username and try again.[/]");
                        IOConsole.PauseOutput();
                    }
                    break;
                case CUSTOMER:
                    username = IOConsole.GetUsername(CUSTOMER);
                    password = IOConsole.GetPassword(CUSTOMER);
                    if (this.VerifyCustomer(username, password))
                    {
                        IOConsole.WriteLoginSuccess(username, ADMIN);
                        CustomerController cc = new CustomerController(Context, CustomerDao.GetCustomerByUsername(Context, username));
                        cc.Run();
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Please check your password and username and try again.[/]");
                        IOConsole.PauseOutput();
                    }
                    break;
                case EXIT:
                    if (AnsiConsole.Confirm("Exit the application?"))
                    {
                        isRunning = false;
                    }
                    break;
            }
        }
    }

    private bool VerifyCustomer(string username, string password)
    {
        return PasswordUtils.VerifyCustomer(username, password);
    }

    private bool VerifyAdmin(string username, string password)
    {
        return PasswordUtils.VerifyAdmin(username, password);
    }

}