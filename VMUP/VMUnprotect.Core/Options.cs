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

        string targetDirectory = Path.GetDirectoryName(TargetFile)!;
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(TargetFile);
        string directory = Path.Combine(targetDirectory, $"{fileNameWithoutExtension}-patched");

        if (!Directory.Exists(directory))
        {
            Logger.Debug("Creating directory '{Directory}'", directory);
            Directory.CreateDirectory(directory);
        }

        OutputFile = Path.Combine(directory, $"{fileNameWithoutExtension}{Path.GetExtension(TargetFile)}");
    }

    public ILogger Logger
    {
        get;
    }

    public string TargetFile
    {
        get;
    }

    public string OutputFile
    {
        get;
    }
}
