using System;

namespace VMUnprotect.Core.Abstraction {
    public class EmptyLogger : ILogger {
        public void Debug(string m, params object[] f) {
            throw new NotImplementedException();
        }
        public void Error(string m, params object[] f) {
            throw new NotImplementedException();
        }
        public void Info(string m, params object[] f) {
            throw new NotImplementedException();
        }
        public void Warn(string m, params object[] f) {
            throw new NotImplementedException();
        }
        public void Print(string m, params object[] f) {
            throw new NotImplementedException();
        }
    }
}