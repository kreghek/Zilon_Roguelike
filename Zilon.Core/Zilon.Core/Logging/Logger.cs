namespace Zilon.Core.Logging
{
    using System;

    public static class Logger
    {
        public static void TraceInfo(LogCodes code, string message, Exception exception = null)
        {
            TraceInner(() =>
            {
                
                Console.WriteLine("{0} {1}: {2}", "Info", code.ToString(), message);
                if (exception != null)
                {
                    Console.WriteLine(exception.ToString());
                }
            });
        }

        public static void TraceWarning(LogCodes code, string message, Exception exception = null)
        {
            TraceInner(() =>
            {
                
                Console.WriteLine("{0} {1}: {2}", "Info", code.ToString(), message);
                if (exception != null)
                {
                    Console.WriteLine(exception.ToString());
                }
            });
        }

        public static void TraceError(LogCodes code, string message, Exception exception = null)
        {
            TraceInner(() =>
            {
                
                Console.WriteLine("{0} {1}: {2}", "Info", code.ToString(), message);
                if (exception != null)
                {
                    Console.WriteLine(exception.ToString());
                }
            });

#if DEBUG
            throw new Exception(message, exception);
#endif
        }

        public static void Close()
        {
            TraceInfo(LogCodes.TraceCommon, "Выгрузка логирования");
        }

        private static void TraceInner(Action logDelegate)
        {
            logDelegate();
        }
    }
}
