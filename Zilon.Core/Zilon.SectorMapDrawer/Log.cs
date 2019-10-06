using System;

namespace Zilon.SectorGegerator
{
    public static class Log
    {
        public static void Info(string message) {
            Console.WriteLine(message);
        }

        public static void Error(Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }

        public static void Error(string message)
        {
            Console.WriteLine(message);
        }
    }
}
