using VMUnprotect.Runtime.General;

namespace VMUnprotect.Runtime.Helpers
{
    public abstract class Params
    {
        internal static Context Ctx;
        internal static ILogger Logger;

        protected Params(Context ctx, ILogger logger) {
            Ctx = ctx;
            Logger = logger;

        }
    }
}