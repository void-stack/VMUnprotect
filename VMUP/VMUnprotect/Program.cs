using System.CommandLine;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using VMUnprotect.Core;

namespace VMUnprotect;

internal static class Program
{
    public static int Main(string[] args)
    {
        var fileArgument = new Argument<string>(
            "file",
            "The input module to obfuscate.");

        var verbosityOption = new Option<LogEventLevel>(
            "--v",
            () => LogEventLevel.Information,
            "Set the verbosity level ");

        var rootCommand = new RootCommand
        {
            fileArgument,
            verbosityOption
        };

        rootCommand.SetHandler(ConstructOptions, fileArgument, verbosityOption);
        return rootCommand.Invoke(args);
    }

    private static void ConstructOptions(string file, LogEventLevel verbosity)
    {
        Ascii.ShowInfo();

        var logger = new LoggerConfiguration()
            .MinimumLevel.Is(verbosity)
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .CreateLogger();

        logger.Debug("Constructing options...");
        new Project(new Options(file, logger)).Run();
    }
}
