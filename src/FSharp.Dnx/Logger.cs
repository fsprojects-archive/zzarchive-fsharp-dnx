using System;

namespace FSharp.Dnx
{
    internal static class Logger
    {
        public static void TraceError(string message, params object[] args)
        {
            if (IsEnabled)
            {
                Console.WriteLine("Error: " + message, args);
            }
        }

        public static void TraceInformation(string message, params object[] args)
        {
            if (IsEnabled)
            {
                Console.WriteLine("Information: " + message, args);
            }
        }

        public static void TraceWarning(string message, params object[] args)
        {
            if (IsEnabled)
            {
                Console.WriteLine("Warning: " + message, args);
            }
        }

        public static bool IsEnabled => Environment.GetEnvironmentVariable("DNX_TRACE") == "1";
    }
}
