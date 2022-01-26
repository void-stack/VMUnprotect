namespace VMUnprotect.Core.Abstraction {
    public interface ILogger {
        public void Debug(string m, params object[] f);
        public void Error(string m, params object[] f);
        public void Info(string m, params object[] f);
        public void Warn(string m, params object[] f);
        public void Print(string m, params object[] f);
    }
}