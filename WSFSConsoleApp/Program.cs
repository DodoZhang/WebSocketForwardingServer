namespace WSFS.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string location = args[0];
            string? certificate = args.Length >= 2 ? args[1] : null;

            using (WSFSServer server = new(location, certificate))
            {
                server.Start();

                string? input = Console.ReadLine();
                while (input != "stop") input = Console.ReadLine();
            }
        }
    }
}
