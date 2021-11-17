using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using System;

namespace VMUnprotect.Utils
{
    public static class ConsoleLogger
    {
        private static readonly Logger Logger = new LoggerConfiguration().WriteTo
            .File("VMUnprotect.txt", outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}", rollOnFileSizeLimit: true)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}", theme: AnsiConsoleTheme.Grayscale)
            .MinimumLevel.Verbose()
            .CreateLogger();

        public static void Banner(string asciiArt)
        {
            Console.Title = "VMUnprotect for Ultimate v3.5.1";
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Info(asciiArt);
            Info("Made with love by void-stack <3");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Debug(string m, params object[] f)
        {
            Logger.Debug(m, f);
        }
        public static void Error(string m, params object[] f)
        {
            Logger.Error(m, f);
        }
        public static void Info(string m, params object[] f)
        {
            Logger.Information(m, f);
        }
        public static void Warn(string m, params object[] f)
        {
            Logger.Warning(m, f);
        }
    }
}