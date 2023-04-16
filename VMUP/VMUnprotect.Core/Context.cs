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

        Logger.Debug("Running with with options {@Options}", options);
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

        var nativeWriterOptions = new NativeModuleWriterOptions(Module, true)
        {
            Logger = DummyLogger.NoThrowInstance,
            MetadataOptions = { Flags = MetadataFlags.PreserveAll }
        };

        Module.NativeWrite(Options.OutputFile, nativeWriterOptions);
        Logger.Information("Successfully wrote patched file to '{NewFilePath}'", Options.OutputFile);
    }
}
