using dnlib.DotNet;
using dnlib.DotNet.Writer;
using ILogger = Serilog.ILogger;

namespace VMUnprotect.Core;

public class Context
{
    public Context(Options options)
    {
        Options = options;
        Module = ModuleDefMD.Load(options.TargetFile);
    }

    public ModuleDefMD Module
    {
        get;
    }

    public Options Options
    {
        get;
    }

    public ILogger Logger
    {
        get { return Options.Logger; }
    }

    public void WriteToDisk()
    {
        Logger.Information("Writing project...");
        string targetDirectory = Path.GetDirectoryName(Options.TargetFile)!;
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(Options.TargetFile);

        string directory = Path.Combine(targetDirectory, $"{fileNameWithoutExtension}-patched");
        string newFilePath =
            Path.Combine(directory, $"{fileNameWithoutExtension}{Path.GetExtension(Options.TargetFile)}");

        if (!Directory.Exists(directory))
        {
            Logger.Debug("Creating directory '{Directory}'", directory);
            Directory.CreateDirectory(directory);
        }

        var nativeWriterOptions = new NativeModuleWriterOptions(Module, true)
        {
            Logger = DummyLogger.NoThrowInstance,
            MetadataOptions = { Flags = MetadataFlags.PreserveAll }
        };

        Module.NativeWrite(newFilePath, nativeWriterOptions);
        Logger.Information("Successfully wrote patched file to '{NewFilePath}'", newFilePath);
    }
}
