using CommandLine;
using System;
using System.IO;
using System.Linq;
using VMUnprotect.Runtime.General;
using VMUnprotect.Utils;

// https://open.spotify.com/playlist/4IeI5PQYePhXaezV9HRDIr
// ^ Thanks for this banger soundtrack, without it the whole project wouldn't exist

namespace VMUnprotect
{
    internal static class Program
    {
        private const string AsciiArt = @"
       	 ____   ____  ____    ____  _____  _____  _______   
       	|_  _| |_  _||_   \  /   _||_   _||_   _||_   __ \  
       	 \ \   / /    |   \/   |    | |    | |    | |__) | 
       	  \ \ / /     | |\  /| |    | '    ' |    |  ___/  
       	   \ ' /     _| |_\/_| |_    \ \__/ /    _| |_     
       	    \_/     |_____||_____|    `.__.'    |_____|
             https://github.com/void-stack/VMUnprotect
       	     VMUnprotect Ultimate v 3.6.0                     
        ";

        public static void Main(string[] args) {
            Console.Title = "VMUnprotect Ultimate";
            ConsoleLogger.Banner(AsciiArt);

            if (args.Length > 0 && File.Exists(args[0]))
                args[0] = $"-f {args[0]}";

            Parser.Default.ParseArguments<CommandLineOptions>(args)
                  .WithParsed(options => {
                      var fileName = Path.GetFileNameWithoutExtension(options.FilePath);
                      var fullPath = Path.GetFullPath(options.FilePath!);

                      var logger = new ConsoleLogger(fileName);
                      logger.Info(
                          "Doesn't work? Make sure you dump the file before with: https://github.com/void-stack/VMUnprotect.Dumper");

                      var project = new Project {
                          TargetFilePath = fullPath
                      };

                      try {
                          project.Run(logger, options);
                      } catch (Exception ex) {
                          // ignored
                      }

                      Console.ReadKey();
                  })
                  .WithNotParsed(errors => {
                      Console.WriteLine("Errors: {0}", string.Join(", ", errors.Select(ex => ex.Tag)));
                      Console.ReadKey();
                  });

            Environment.Exit(0);
        }
    }
}