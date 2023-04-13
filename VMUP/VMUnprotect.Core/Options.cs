using Serilog;

namespace VMUnprotect.Core;

public class Options
{
    public Options(string targetFile, ILogger logger)
    {
        if (!File.Exists(targetFile))
        {
            logger.Error("Target file does {File} not exist!", targetFile);
            Environment.Exit(1);
        }

        TargetFile = targetFile;
        Logger = logger;
    }

    public ILogger Logger
    {
        get;
    }

    public string TargetFile
    {
        get;
    }
}
