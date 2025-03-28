using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.Utilities
{
    public static class Logger
    {
        public static void Log(string message)
        {
            Trace.WriteLine(message);
        }

        public static void Log(string tag, string message)
        {
            Trace.WriteLine($"{tag}: {message}");
        }

        public static void LogError(string error)
        {
            Trace.WriteLine($"ERROR: {error}");
        }

        public static void LogWarning(string warning)
        {
            Trace.WriteLine($"WARNING: {warning}");
        }
    }
}
