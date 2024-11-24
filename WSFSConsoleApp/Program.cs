using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Fleck;

namespace WSFS.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string location;
            if (args.Length >= 1)
            {
                location = args[0];
            }
            else
            {
                FleckLog.Error("Missing location parameter. Start the application with e.g. \"wsfs ws:/0.0.0.0:8080\" command.");
                return;
            }

            X509Certificate2? certificate = null;
            if (args.Length >= 2)
            {
                try
                {
                    certificate = new X509Certificate2(args[1]);
                }
                catch (CryptographicException ex)
                {
                    FleckLog.Warn("Failed to load certification file.", ex);
                }
            }

            using (WSFSServer server = new(location, certificate))
            {
                server.Behaviour = new WSFSBehaviourWithPassword();
                server.Start();

                string? input = Console.ReadLine();
                while (input != "stop") input = Console.ReadLine();
            }
        }
    }
}
