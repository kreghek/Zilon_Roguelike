namespace Zilon.Core.MassSectorGenerator
{
    public static class Log
    {
        public static void Info(string message)
        {
            Console.WriteLine(message);
        }

        public static void Error(Exception exception)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            Console.WriteLine(exception.ToString());
        }

        public static void Error(string message)
        {
            Console.WriteLine(message);
        }
    }
}