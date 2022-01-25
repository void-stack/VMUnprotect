using CommandLine;
using System;
using System.IO;
using System.Linq;
using VMUnprotect.Init;
using VMUnprotect.Utils;

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
       	     VMUnprotect Ultimate v 3.5.1                     
        ";

        public static void Main(string[] args)
        {
            ConsoleLogger.Banner(AsciiArt);
            Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsed(options =>
            {
                var file = Path.GetFullPath(options.FilePath);
            
                if (!File.Exists(file))
                {
                    ConsoleLogger.Error($"{file} is not a file or it does not exist");
                    Console.ReadLine();
                    return;
                }
                new Loader(file).Start(options);
            
                Console.ReadKey();
            })
            .WithNotParsed(errors =>
            {
                Console.WriteLine("Errors: {0}", string.Join(", ", errors.Select(ex => ex.Tag)));
                Console.ReadKey();
            });
            Environment.Exit(0);
        }
    }
}