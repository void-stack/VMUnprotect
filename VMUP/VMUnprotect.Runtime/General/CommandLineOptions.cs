using CommandLine;

namespace VMUnprotect.Runtime.General
{
    public class CommandLineOptions
    {
        [Option('f', "file", HelpText = "Path to file.", Required = true)]
        public string FilePath { get; set; } = null!;

        /*[Option(Default = false, HelpText = "Use an older method that makes use of Transpiler (not recommended).",
                Required = false)]
        public bool UseTranspiler { get; set; }*/

        [Option(Default = false, HelpText = "Disable or Enable logs from Harmony.", Required = false)]
        public bool EnableHarmonyLogs { get; set; }

        [Option(Default = false, HelpText = "Bypass VMProtect Anti Debug.", Required = false)]
        public bool BypassAntiDebug { get; set; }
    }
}