
namespace SimEi.Obfuscator
{
    internal static class Logger
    {
        public static void Log(string message, bool printTimestamp = true)
        {
            if (printTimestamp)
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            else
                Console.WriteLine(message);
        }
    }
}
