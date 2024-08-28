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
            MENU_NAME = "***************************\n" +
                    $"Admin Menu - {admin.AdminUsername}\n" +
                    "***************************";
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
        AnsiConsole.Clear();
        MENU_NAME = "****************"
                + "\nDatabase Summary"
                + "\n****************";

        IOConsole.WriteMenu(MENU_NAME);
        var tableCount = new Table();
        tableCount.AddColumn("Table");
        tableCount.AddColumn("Count");

        const string ADMINS = "Administrators", CUSTOMERS = "Customers", ACCOUNTS = "Accounts", TRANSACTION_LOGS = "Transactions Logs", REQUESTS = "Requests";

        int adminCount = Context.Admins.Count();
        int customerCount = Context.Customers.Count();
        int accountCount = Context.Accounts.Count();
        int transactionCount = Context.TransactionLogs.Count();
        int requestCount = Context.Requests.Count();

        tableCount.AddRow(ADMINS, adminCount.ToString());
        tableCount.AddRow(CUSTOMERS, customerCount.ToString());
        tableCount.AddRow(ACCOUNTS, accountCount.ToString());
        tableCount.AddRow(TRANSACTION_LOGS, transactionCount.ToString());
        tableCount.AddRow(REQUESTS, requestCount.ToString());

        AnsiConsole.Write(tableCount);
        AnsiConsole.WriteLine();

        var tableAverage = new Table();
        tableAverage.AddColumn("Average Account Balance");
        tableAverage.AddColumn("Average Transaction Balance");

        decimal balanceAvg = Context.Accounts.Average(a => a.Balance);
        decimal transactionAvg = Context.TransactionLogs.Average(t => t.Amount);

        tableAverage.AddRow("$" + balanceAvg.ToString(), "$" + transactionAvg.ToString());
        AnsiConsole.Write(tableAverage);
    }

    private void UpdateAccount()
    {
        AnsiConsole.Clear();
        MENU_NAME = "****************"
                  + "\nUpdate Account"
                  + "\n**************";
        IOConsole.WriteMenu(MENU_NAME);
        var account = AccountDao.GetAccountById(Context, IOConsole.GetAccountId());

        if (account == null)
        {
            AnsiConsole.MarkupLine("[red]There is no Account with that ID number.[/]");
        }

        else
        {
            UpdateAccount(account);
        }

    }

    private void UpdateAccount(Account account)
    {
        AnsiConsole.Clear();
        const string TYPE = "Account Type", IS_ACTIVE = "Active Status", BALANCE = "Balance", CANCEL = "Cancel";
        MENU_NAME = "*******************************************************\n"
                  + $"Update Account No. {account.AccId} for Customer {account.Customer.CustomerUsername}\n"
                  + "*********************************************************";
        IOConsole.WriteMenu(MENU_NAME);
        var menu = new SelectionPrompt<string>()
                    .Title("Use the arrow keys to select an option")
                  .PageSize(10)
                  .HighlightStyle(new Style(foreground: Color.Green, background: Color.Black))
                  .AddChoices(new[] {
                        TYPE,
                        IS_ACTIVE,
                        BALANCE,
                        CANCEL,
                  });

        var choice = AnsiConsole.Prompt(menu);
        switch (choice)
        {
            case TYPE:
                UpdateAccountType(account);
                break;
            case IS_ACTIVE:
                UpdateIsActive(account);
                break;
            case BALANCE:
                UpdateBalance(account);
                break;
            case CANCEL:
                if (AnsiConsole.Confirm("Cancel account update?"))
                {
                    AnsiConsole.WriteLine("Cancelling account update. Press any key to reutn to the Admin menu...");

                    return;
                }
                break;
        }
    }

    private void UpdateBalance(Account account)
    {
        decimal beforeBalance = account.Balance;
        var amount = IOConsole.GetAmount();
        if (AnsiConsole.Confirm($"Set account balance to ${amount}?"))
        {
            AccountDao.UpdateAccountBalance(Context, account, amount);
            TransactionLogDao.CreateTransactionLog(Context, account, amount, "Adjustment");
            AnsiConsole.MarkupLine($"[blue]Balance Before: ${beforeBalance}[/]");
            AnsiConsole.MarkupLine($"[blue]Balance After: ${account.Balance}[/]");

        }
        else
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[yellow]Balance was not updated. Press any key to return to the menu...[/]");

        }
    }

    private void UpdateIsActive(Account account)
    {
        bool isActive = AnsiConsole.Confirm("Is the Account Active?");
        account.IsActive = isActive;
        Context.SaveChanges();
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[blue]Successfully updated Is Active: {account.IsActive}.[/]");
    }

    private void UpdateAccountType(Account account)
    {
        AnsiConsole.Clear();

        const string CHECKING = "Checking", SAVINGS = "Savings", LOAN = "Loan";
        MENU_NAME = "*******************\n"
                + "\nUpdate Account Type"
                + "\n*******************";

        IOConsole.WriteMenu(MENU_NAME);

        var menu = new SelectionPrompt<string>()
                    .Title("Use the arrow keys to update the account type")
                    .HighlightStyle(new Style(foreground: Color.Green, background: Color.Black))
                    .PageSize(10)
                    .AddChoices(new[] {
                       CHECKING,
                       SAVINGS,
                       LOAN
                    });

        var choice = AnsiConsole.Prompt(menu);
        AnsiConsole.WriteLine();
        if (AnsiConsole.Confirm($"Set the account type to {choice}?"))
        {
            AccountDao.UpdateAccountType(Context, account, choice);
            AnsiConsole.MarkupLine($"Account type was set to {choice}! Press any key to return to the menu.");

        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]The account type was not updated. Press any key to return to the menu...[/]");

        }
    }

    private void DeleteAccount()
    {
        MENU_NAME = "**************\n"
                  + "Delete Account\n"
                  + "**************\n";

        var username = IOConsole.GetUsername("Customer");
        Customer customer = CustomerDao.GetCustomerByUsername(Context, username);
        if (customer == null)
        {
            AnsiConsole.MarkupLine($"[red]A customer with that username could not be found.");
        }
        else
        {
            List<Account> accountList = AccountDao.GetAllAccountsByUsername(Context, customer);
            IOConsole.WriteAllAccountDetails(accountList);
            var accountId = IOConsole.GetAccountId();
            Account account = AccountDao.GetAccountById(Context, accountId);
            if (account == null || !accountList.Contains(account))
            {
                AnsiConsole.MarkupLine("[red]An account with that ID could not be found for this user.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]WARNING: Account deletion will also delete all related transactions and requests.[/]");
                AnsiConsole.WriteLine();
                if (AnsiConsole.Confirm("Are you sure you want to delete this account?"))
                {
                    TransactionLogDao.DeleteAllTransactionsForAccount(Context, account);
                    AccountDao.DeleteAccount(Context, account);
                    AnsiConsole.MarkupLine("[blue]The account was successfully deleted.[/]");
                }
            }
        }
    }

    private void CreateAccount()
    {
        AnsiConsole.Clear();

        MENU_NAME = "********************"
                  + "Create a New Account\n"
                  + "********************";
        
        const string CHECKING = "Checking", SAVINGS = "Savings", LOAN = "Loan", CANCEL = "Cancel";
        var menu = new SelectionPrompt<string>()
                    .Title("Use the arrow keys to select an Account Type")
                  .PageSize(5)
                  .AddChoices(new[] {
                    CHECKING,
                    SAVINGS,
                    LOAN,
                    CANCEL
                  });


        IOConsole.WriteMenu(MENU_NAME);
        var username = IOConsole.GetUsername("Customer");
        Customer? customer = Context.Customers.SingleOrDefault(c => c.CustomerUsername == username);
        if (customer == null)
        {
            AnsiConsole.MarkupLine($"[red]{username} was not found.");
            if (AnsiConsole.Confirm($"Add {username} as a customer?"))
            {
                string defaultPassword = "password1";
                byte[] salt = PasswordUtils.GenerateSalt();
                customer = new Customer()
                {
                    CustomerUsername = username,
                    PasswordHash = PasswordUtils.HashPassword(defaultPassword, salt),
                    Salt = salt
                };

                CustomerDao.CreateCustomer(Context, customer);
                AnsiConsole.MarkupLine($"[blue]{username} was successfully added!");
                IOConsole.PauseOutput();
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Cancelling account creation and returning to Admin menu...[/]");
                
                return;
            }
        }

        var choice = AnsiConsole.Prompt(menu);
        switch (choice)
        {
            case CANCEL:
                if (AnsiConsole.Confirm("Cancel account creation?"))
                {
                    AnsiConsole.MarkupLine("[yellow]Cancelling account creating and returning to Admin menu...[/]");
                    return;
                }
                break;
        }

        if (AnsiConsole.Confirm($"Create a {choice} account for {username}?"))
        {

            Account account = new Account()
            {
                CustomerId = customer.CustomerId,
                AccType = choice,
                Balance = 0m,
                IsActive = true
            };

            AccountDao.CreateAccount(Context, account);
            AnsiConsole.MarkupLine("[blue]Successfully created the account![/]");
            IOConsole.WriteAllAccountDetails(account);
            
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Cancelling account creating and returning to Admin menu...[/]");
        }
    }
}
