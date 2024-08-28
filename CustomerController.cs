using System.Diagnostics.CodeAnalysis;
using System.Security.AccessControl;
using Microsoft.SqlServer.Server;
using P0_brendan_BankingApp_Final.POCO;
using Spectre.Console;

public class CustomerController
{
    Customer customer;
    P0BrendanBankingDbContext Context;
    string MENU_NAME = "";

    public CustomerController(P0BrendanBankingDbContext context, Customer customer)
    {
        Context = context;
        this.customer = customer;
    }

    public void Run()
    {
        const string CHECK_DETAILS = "Check Account Details",
                    WITHDRAW = "Withdraw Funds",
                    DEPOSIT = "Deposit Funds",
                    TRANSFER = "Transfer Funds",
                    HISTORY = "View Last 5 Transactions",
                    RESET = "Reset Password",
                    REQUEST = "Request a New Checkbook",
                    EXIT = "Exit to Main Menu";

        var menu = new SelectionPrompt<string>()
                    .Title("Use the arrow keys to select an option")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Green, background: Color.Black))
                    .AddChoices(new[] {
                            CHECK_DETAILS,
                            WITHDRAW,
                            DEPOSIT,
                            TRANSFER,
                            HISTORY,
                            RESET,
                            REQUEST,
                            EXIT
                    });

        bool isRunning = true;
        while (isRunning)
        {
            AnsiConsole.Clear();
            MENU_NAME = "*****************************\n"
                      + $"{customer.CustomerUsername} - Main Menu\n"
                      + "*****************************";

            IOConsole.WriteMenu(MENU_NAME);
            
            var choice = AnsiConsole.Prompt(menu);
            switch (choice)
            {
                case CHECK_DETAILS:
                    CheckDetails();
                    break;
                case WITHDRAW:
                    WithdrawFunds();
                    break;
                case DEPOSIT:
                    DepositFunds();
                    break;
                case TRANSFER:
                    TransferFunds();
                    break;
                case HISTORY:
                    ViewHistory();
                    break;
                case RESET:
                    ResetPassword();
                    break;
                case REQUEST:
                    RequestCheckbook();
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

    private void RequestCheckbook()
    {
        AnsiConsole.Clear();
        MENU_NAME = "*****************\n"
                  + "Request Checkbook\n"
                  + "*****************\n";
        IOConsole.WriteMenu(MENU_NAME);

        Account account = AccountDao.GetAccountById(Context, IOConsole.GetAccountId());
        if (account == null)
        {
            AnsiConsole.MarkupLine("[red]Cannot find an account with that ID.[/]");
            return;
        }
        else if (account.AccType == "Savings" || account.AccType == "Loan")
        {
            AnsiConsole.MarkupLine("[red]You can only request checkbooks for checking accounts.[/]");
            return;
        }
        else if (account.CustomerId != customer.CustomerId)
        {
            AnsiConsole.MarkupLine("[red]This account does not belong to you. Cannot request checkbook.[/]");
            return;
        }

        const int DEFAULT_ADMIN_ID = 2; // Right now only one admin exists in the database. AdminId is 2

        Request request = new Request()
        {
            CustomerId = customer.CustomerId,
            AccId = account.AccId,
            AdminId = DEFAULT_ADMIN_ID,
            RequestType = "CHECKBOOK",
            RequestDate = DateTime.Now,
            Status = "OPEN"
        };

        RequestDao.CreateRequest(Context, request);
        AnsiConsole.MarkupLine("[blue]The checkbook request sent to an administrator.[/]");
    }

    private void ResetPassword()
    {
        const string OLD = "your old", NEW = "your new", AGAIN = "(again)";
        AnsiConsole.Clear();
        MENU_NAME = "**************\n"
                  + "Reset Password\n"
                  + "**************\n";
        IOConsole.WriteMenu(MENU_NAME);
        var oldPassword = IOConsole.GetPassword(OLD);

        if (!PasswordUtils.VerifyCustomer(customer.CustomerUsername, oldPassword))
        {
            AnsiConsole.MarkupLine("[red]Your password was incorrect.[/]");
            return;
        }

        var newPassword1 = IOConsole.GetPassword(NEW);
        var newPassword2 = IOConsole.GetPassword(AGAIN + " " + NEW);

        if (newPassword1 != newPassword2)
        {
            AnsiConsole.MarkupLine("[red]Your passwords do not match.[/]");
            return;
        }

        CustomerDao.ResetPassword(Context, customer, newPassword1);
        AnsiConsole.MarkupLine("[blue]Your password was successfully changed![/]");
    }

    private void ViewHistory()
    {
        AnsiConsole.Clear();
        const int LIMIT = 5;
        MENU_NAME = "*******************\n"
                  + "Last 5 Transactions\n"
                  + "*******************\n";
        IOConsole.WriteMenu(MENU_NAME);

        List<TransactionLog> recentTransactions = TransactionLogDao.GetTransactionLogs(Context, customer, LIMIT);

        foreach (var tl in recentTransactions)
        {
            AnsiConsole.WriteLine();
            IOConsole.WriteTransactionLogDetails(tl);
            AnsiConsole.WriteLine();

        }
    }

    private void TransferFunds()
    {
        AnsiConsole.Clear();
        MENU_NAME = "*************\n"
                  + "Transfer Funds\n"
                  + "*************\n";
        IOConsole.WriteMenu(MENU_NAME);

        AnsiConsole.WriteLine("1. Enter the Account ID you are transferring funds from.");
        Account fromAccount = AccountDao.GetAccountById(Context, IOConsole.GetAccountId());
        if (fromAccount == null)
        {
            AnsiConsole.MarkupLine("[red]The account does not exist...[/]");
            return;
        }
        else if (fromAccount.CustomerId != customer.CustomerId)
        {
            AnsiConsole.MarkupLine("[red]Cannot perform transfer because the from account doesn't belong to you.[/]");
            return;
        }
        else if (fromAccount.AccType == "Loan")
        {
            AnsiConsole.MarkupLine("[red]Cannot perform transfer because the from account is a loan.[/]");
            return;
        }
        else if (!fromAccount.IsActive)
        {
            AnsiConsole.MarkupLine("[red]Cannot perform transfer because the sending account is not active[/]");
            return;
        }

        AnsiConsole.WriteLine("2. Enter the Account Id you are transferring the funds to.");
        Account toAccount = AccountDao.GetAccountById(Context, IOConsole.GetAccountId());
        if (toAccount == null)
        {
            AnsiConsole.MarkupLine("[red]The account does not exist...[/]");
            return;
        }
        else if (!fromAccount.IsActive)
        {
            AnsiConsole.MarkupLine("[red]Cannot perform transfer because the receiving account is not active[/]");
            return;
        }
        else if (toAccount.CustomerId != customer.CustomerId)
        {
            if (!AnsiConsole.Confirm("The account does not belong to you. Continue?"))
            {
                AnsiConsole.MarkupLine("[yellow]Cancelling the transfer...[/]");
                return;
            }
        }

        var amount = IOConsole.GetAmount();

        if (amount > fromAccount.Balance)
        {
            AnsiConsole.MarkupLine("[red]Cannot withdraw an amount greater than total From-Account Balance.[/]");
            return;
        }

        AccountDao.WithdrawFunds(Context, fromAccount, amount);
        AccountDao.DepositFunds(Context, toAccount, amount);
        TransactionLogDao.CreateTransactionLog(Context, fromAccount, amount, "Transfer");
        TransactionLogDao.CreateTransactionLog(Context, toAccount, amount, "Transfer");

        AnsiConsole.MarkupLine("[blue]Transfer completed![/]");
        AnsiConsole.WriteLine("Sending Account");
        IOConsole.WritePartialAccountDetails(fromAccount);
        AnsiConsole.WriteLine("Receiving Account");
        IOConsole.WritePartialAccountDetails(toAccount);
    }

    private void DepositFunds()
    {
        AnsiConsole.Clear();
        MENU_NAME = "*************\n"
                  + "Deposit Funds\n"
                  + "*************";
        IOConsole.WriteMenu(MENU_NAME);

        var account = AccountDao.GetAccountById(Context, IOConsole.GetAccountId());
        if (account == null)
        {
            AnsiConsole.MarkupLine("[red]The account does not exist...[/]");
            return;

        }
        else if (account.CustomerId != customer.CustomerId)
        {
            if (!AnsiConsole.Confirm("The account does not belong to you. Continue?"))
            {
                AnsiConsole.MarkupLine("[yellow]Cancelling the deposit...[/]");
                return;
            }

        }
        else if (!account.IsActive)
        {
            AnsiConsole.MarkupLine("[red]Cannot deposit funds because the account is not active[/]");
            return;
        }

        var amount = IOConsole.GetAmount();
        AccountDao.DepositFunds(Context, account, amount);
        TransactionLogDao.CreateTransactionLog(Context, account, amount, "Deposit");
        AnsiConsole.MarkupLine($"[blue]Your new account balance: ${account.Balance}[/]");
    }

    private void WithdrawFunds()
    {
        AnsiConsole.Clear();
        MENU_NAME = "**************\n"
                  + "Withdraw Funds\n"
                  + "**************";
        IOConsole.WriteMenu(MENU_NAME);

        var account = AccountDao.GetAccountById(Context, IOConsole.GetAccountId());
        if (account == null)
        {
            AnsiConsole.MarkupLine("[red]The account does not exist...[/]");
            return;

        }
        else if (account.CustomerId != customer.CustomerId)
        {
            AnsiConsole.MarkupLine("[red]Cannot perform withdraw because the account doesn't belong to you.[/]");
            return;
        }

        else if (account.AccType == "Loan")
        {
            AnsiConsole.MarkupLine("[red]Cannot withdraw from this account type.[/]");
            return;
        }
        else if (!account.IsActive)
        {
            AnsiConsole.MarkupLine("[red]Cannot withdraw funds because the account is not active[/]");
            return;
        }

        var amount = IOConsole.GetAmount();

        if (amount > account.Balance)
        {
            AnsiConsole.MarkupLine("[red]Cannot withdraw an amount greater than total Account Balance.[/]");
            return;
        }

        AccountDao.WithdrawFunds(Context, account, amount);
        TransactionLogDao.CreateTransactionLog(Context, account, amount, "Withdraw");

        AnsiConsole.MarkupLine($"[blue]Your new account balance: ${account.Balance}[/]");
    }

    private void CheckDetails()
    {
        AnsiConsole.Clear();
        MENU_NAME = "*******************\n"
                  + "All Account Details\n"
                  + "*******************";
        IOConsole.WriteMenu(MENU_NAME);

        var allAccounts = AccountDao.GetAllAccountsByUsername(Context, customer);

        foreach (var account in allAccounts)
        {
            AnsiConsole.WriteLine();
            IOConsole.WritePartialAccountDetails(account);
            AnsiConsole.WriteLine();
        }
    }
}
