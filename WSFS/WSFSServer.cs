using Fleck;
using System.Security.Cryptography.X509Certificates;

namespace WSFS
{
    public class WSFSServer : IDisposable
    {
        private WebSocketServer server;

        /// <summary>
        /// How server behaves when Domain or Connection events triggers.<br/>
        /// Override class <c>WSFSBehaviour</c> to design custom behaviours.
        /// </summary>
        public WSFSBehaviour Behaviour { get; set; } = new();

        public IReadOnlySet<WSFSConnection> Connections => connections;
        internal HashSet<WSFSConnection> connections = new();

        public IReadOnlyDictionary<string, WSFSDomain> Domains => domains;
        internal Dictionary<string, WSFSDomain> domains = new();

        public WSFSServer(string location, string? certificate = null)
        {
            server = new WebSocketServer(location);
            if (certificate is not null) server.Certificate = new X509Certificate2(certificate);
        }

        public void Start()
        {
            server.Start(connection => new WSFSConnection(this, connection));
        }

        public void Dispose()
        {
            foreach (WSFSConnection connection in connections) connection.Close();
            server.Dispose();
        }
    }
}
