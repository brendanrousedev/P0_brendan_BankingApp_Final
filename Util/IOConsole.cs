using P0_brendan_BankingApp_Final.POCO;
using Spectre.Console;

public static class IOConsole
{
    public static void WriteMenu(string menu)
    {
        AnsiConsole.MarkupLine($"[blue]{menu}[/]\n\n");
    }

    public static void WriteGreeting()
    {
        AnsiConsole.MarkupLine("[blue]Welcome to The Bank of Arstotzka![/]");
    }

    public static void WriteFarewell()
    {
        AnsiConsole.MarkupLine("[blue]Thankyou for using The Bank of Arstotzka application[/]");
        AnsiConsole.MarkupLine("[blue]The program is now closing....[/]");
        PauseOutput();
    }

    public static void PauseOutput()
    {
        AnsiConsole.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    public static string GetPassword(string userType)
    {
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>($"Enter {userType} password:")
                .PromptStyle("green")
                .Secret('*')
                .Validate(input =>
                {
                    return input.Length > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Password cannot be empty[/]");
                }));

        return password;
    }

    public static int GetAccountId()
    {
        var accountId = AnsiConsole.Prompt(
            new TextPrompt<decimal>("What is the Account ID?")
                        .PromptStyle("green")
                        .ValidationErrorMessage("[red]That's not a valid Id.[/]")
                        .Validate(amount =>
                        {
                            return amount switch
                            {
                                _ => ValidationResult.Success(),

                            };
                        }));
        return (int) accountId;
    }

    public static string GetUsername(string userType)
    {
        var username = AnsiConsole.Prompt(
            new TextPrompt<string>($"Enter {userType} username:")
                .PromptStyle("green")
                .Validate(input =>
                {
                    return input.Length > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Username cannot be empty[/]");
                }));

        return username;
    }

    public static void WriteLoginSuccess(string username, string userType)
    {
        AnsiConsole.Clear();
        string SUCCESS = "**************************************************\n" +
                         $"Successfully Logged in as {userType} {username}\n" +
                         "**************************************************\n";
        WriteMenu(SUCCESS);
        PauseOutput();
    }

    public static decimal GetAmount()
    {
        var amount = AnsiConsole.Prompt(
            new TextPrompt<decimal>("What is the amount? $")
                        .PromptStyle("green")
                        .ValidationErrorMessage("[red]That's not a valid number.[/]")
                        .Validate(amount =>
                        {
                            return amount switch
                            {
                                < 0m => ValidationResult.Error("Amount cannot be negative.[/]"),
                                _ => ValidationResult.Success(),

                            };
                        }));

        return amount;
    }

    public static void WriteAllAccountDetails(List<Account> accounts)
    {
        foreach(var account in accounts)
        {
            WriteAllAccountDetails(account);
        }
    }

    public static void WriteAllAccountDetails(Account account)
    {
        var table = new Table();
        const string ACC_ID = "AccId",
            CUSTOMER_NAME = "CustomerUsername",
            CUSTOMER_ID = "CustomerId",
            ACC_TYPE = "AccType",
            BALANCE = "Balance",
            IS_ACTIVE = "Is Active";

        table.AddColumn(ACC_ID);
        table.AddColumn(CUSTOMER_NAME);
        table.AddColumn(CUSTOMER_ID);
        table.AddColumn(ACC_TYPE);
        table.AddColumn(BALANCE);
        table.AddColumn(IS_ACTIVE);

        table.AddRow(account.AccId.ToString(),
                account.Customer.CustomerUsername,
                account.CustomerId.ToString(),
                account.AccType,
                "$" + account.Balance.ToString(),
                account.IsActive.ToString()
        );

        AnsiConsole.Write(table);
    }
}