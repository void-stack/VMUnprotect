using CommandLine;

namespace VMUnprotect.Core {
    public class CommandLineOptions {
        [Option('f', "file", HelpText = "Path to file.", Required = true)]
        public string FilePath { get; set; }

        [Option(Default = false, HelpText = "Use an older method that makes use of Transpiler (not recommended).", Required = false)]
        public bool UseTranspiler { get; set; }

        [Option(Default = true, HelpText = "Enable logs from Harmony.", Required = false)]
        public bool EnableHarmonyLogs { get; set; }
    }
}