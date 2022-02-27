using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using ILogger = VMUnprotect.Runtime.General.ILogger;

namespace VMUnprotect.Utils
{
    public class ConsoleLogger : ILogger
    {
        private readonly Logger _logger;

        public ConsoleLogger(string filename) {
            if (Directory.Exists("VMUP_Logs"))
                Directory.CreateDirectory("VMUP_Logs");
            
            _logger = new LoggerConfiguration().WriteTo.File($"VMUP_Logs\\{filename}_{DateTime.Now:HH-mm-ss}.vmuplog")
                                               .WriteTo.Console(theme: AnsiConsoleTheme.Grayscale)
                                               .MinimumLevel.Verbose()
                                               .CreateLogger();
        }

        public void Debug(string m, params object[] f) {
            _logger.Debug(m, f);
        }

        public void Error(string m, params object[] f) {
            _logger.Error(m, f);
        }

        public void Info(string m, params object[] f) {
            _logger.Information(m, f);
        }

        public void Warn(string m, params object[] f) {
            _logger.Warning(m, f);
        }

        public void Print(string m, params object[] f) {
            _logger.Verbose('\t' + m, f);
        }

        public static void Banner(string asciiArt) {
            Console.Title = "VMUnprotect for Ultimate v3.5.1";
            Console.WriteLine(asciiArt);
            Console.WriteLine("Made with love by void-stack <3");
        }
    }
}