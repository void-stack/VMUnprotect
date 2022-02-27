using System;
using System.IO;

namespace VMUnprotect.Runtime.General
{
    public class Project
    {
        public string TargetFilePath { get; set; }

        public void Run(ILogger logger, CommandLineOptions options) {
            if (!File.Exists(TargetFilePath)) {
                logger.Error($"{TargetFilePath} is not a file or it does not exist");
                Console.ReadLine();
                return;
            }

            Engine.Initialize(this, logger, options);

            
            Engine.Run(this);
        }
    }
}