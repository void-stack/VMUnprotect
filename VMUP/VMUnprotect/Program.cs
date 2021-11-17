using System;
using System.IO;
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
    
       			VMUnprotect Ultimate v 3.5.1                     
        ";

        public static void Main(string[] args)
        {
            ConsoleLogger.Banner(AsciiArt);
            if (args.Length < 1)
            {
                ConsoleLogger.Error("VMUnprotect.exe <path to assembly> [args to assembly]");
                Console.ReadLine();
                return;
            }

            var file = Path.GetFullPath(args[0]);
            if (!File.Exists(file))
            {
                ConsoleLogger.Error($"{file} is not a file or it does not exist");
                Console.ReadLine();
                return;
            }

            new Loader(file).Start(args);
            Console.ReadKey();
        }
    }
}