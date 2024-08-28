using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using P0_brendan_BankingApp_Final.POCO;
using Spectre.Console;
public class Program
{
    P0BrendanBankingDbContext Context;

    const string MENU_TITLE = "**********\n" +
                              "Main Login\n" +
                              "**********\n";

    public Program()
    {
        Context = new P0BrendanBankingDbContext();
    }

    public static void Main(string[] args)
    {
        Program program = new Program();
        program.Run();
    }

    private void Run()
    {
        AnsiConsole.Clear();
        IOConsole.WriteGreeting();
        IOConsole.PauseOutput();
        LoginController lc = new LoginController(Context);
        lc.Run();
        IOConsole.WriteFarewell();
    }
}