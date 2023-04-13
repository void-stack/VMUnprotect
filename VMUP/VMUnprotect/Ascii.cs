namespace VMUnprotect;

public static class Ascii
{
    // puff is sexy
    public static void ShowInfo()
    {
        Console.WriteLine();
        Console.WriteLine();
        WriteLineMiddle(@" ██▒   █▓ ███▄ ▄███▓ █    ██  ██▓███  ", ConsoleColor.DarkMagenta);
        WriteLineMiddle(@"▓██░   █▒▓██▒▀█▀ ██▒ ██  ▓██▒▓██░  ██▒", ConsoleColor.DarkMagenta);
        WriteLineMiddle(@" ▓██  █▒░▓██    ▓██░▓██  ▒██░▓██░ ██▓▒", ConsoleColor.DarkMagenta);
        WriteLineMiddle(@"  ▒██ █░░▒██    ▒██ ▓▓█  ░██░▒██▄█▓▒ ▒", ConsoleColor.DarkMagenta);
        WriteLineMiddle(@"   ▒▀█░  ▒██▒   ░██▒▒▒█████▓ ▒██▒ ░  ░", ConsoleColor.DarkMagenta);
        WriteLineMiddle(@"   ░ ▐░  ░ ▒░   ░  ░░▒▓▒ ▒ ▒ ▒▓▒░ ░  ░", ConsoleColor.DarkMagenta);
        WriteLineMiddle(@"   ░ ░░  ░  ░      ░░░▒░ ░ ░ ░▒ ░     ", ConsoleColor.DarkMagenta);
        Console.WriteLine();

        WriteMiddle(@"Developers - ", ConsoleColor.DarkMagenta);

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("void-stack");
        Console.ResetColor();

        WriteMiddle(@"Github Repo - ", ConsoleColor.DarkMagenta);

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("https://github.com/void-stack/VMUnprotect");
        Console.ResetColor();

        WriteMiddle(@"Thanks to - ", ConsoleColor.DarkMagenta);

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Yck1509 for the inject helper.");

        Console.WriteLine("{0," + (Console.WindowWidth / 2
                                   + 54 + "}"), "Washi1337 for the wonderful AsmResolver library.");
        Console.ResetColor();
    }

    private static void WriteMiddle(object message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write("{0," + (Console.WindowWidth / 2 + message.ToString()?.Length / 2) + "}", message);
        Console.ResetColor();
    }

    private static void WriteLineMiddle(object message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine("{0," + (Console.WindowWidth / 2 + message.ToString()?.Length / 2) + "}", message);
        Console.ResetColor();
    }
}
