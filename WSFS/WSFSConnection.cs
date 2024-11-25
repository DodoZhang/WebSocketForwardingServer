using Fleck;

namespace WSFS
{
    public class WSFSConnection
    {
        public WSFSServer Server { get; private set; }

        private IWebSocketConnection connection;

        /// <summary>
        /// The parameters carried in the url of the Connection.
        /// </summary>
        public IReadOnlyDictionary<string, string> Parameters => parameters;
        private Dictionary<string, string> parameters = new();

        public WSFSDomain? Domain { get; private set; }

        public string ClientIPAddress => connection.ConnectionInfo.ClientIpAddress;
        public int ClientPort => connection.ConnectionInfo.ClientPort;
        public Guid ID => connection.ConnectionInfo.Id;

        internal WSFSConnection(WSFSServer server, IWebSocketConnection connection)
        {
            Server = server;
            this.connection = connection;

            connection.OnOpen = OnOpen;
            connection.OnMessage = OnMessage;
            connection.OnBinary = OnBinary;
            connection.OnClose = OnClose;
        }

        private void OnOpen()
        {
            IWebSocketConnectionInfo info = connection.ConnectionInfo;
            FleckLog.Info(string.Format("Connection {0}:{1} Opened with path {2}.", info.ClientIpAddress, info.ClientPort, info.Path));

            Server.connections.Add(this);

            string path = info.Path;
            int i = path.IndexOf('?');
            string domainPath;
            if (i != -1)
            {
                domainPath = path[..i];
                foreach (string pair in path[(i + 1)..].Split('&'))
                {
                    int j = pair.IndexOf('=');
                    if (j != -1) parameters.Add(pair[..j], pair[(j + 1)..]);
                    else parameters.Add(pair, string.Empty);
                }
            }
            else domainPath = path;

            if (!Server.domains.TryGetValue(domainPath, out WSFSDomain? domain))
                domain = new WSFSDomain(Server, domainPath);
            
            if (Server.Behaviour.OnConnectionJoining(domain, this))
            {
                Domain = domain;
                domain.connections.Add(this);
                FleckLog.Info(string.Format("Connection {0}:{1} Joined Domain {2}.", info.ClientIpAddress, info.ClientPort, domainPath));
            }
            else Close();

            if (domain.connections.Count == 0) domain.Close();
        }

        private void OnMessage(string message)
        {
            if (Domain is null) return;
            foreach (WSFSConnection connection in Domain.connections)
            {
                if (connection == this) continue;
                connection.Send(message);
            }
        }

        private void OnBinary(byte[] message)
        {
            if (Domain is null) return;
            foreach (WSFSConnection connection in Domain.connections)
            {
                if (connection == this) continue;
                connection.Send(message);
            }
        }

        private void OnClose()
        {
            IWebSocketConnectionInfo info = connection.ConnectionInfo;

            if (Domain is not null)
            {
                lock (Domain)
                {
                    Domain.connections.Remove(this);
                    Server.Behaviour.OnConnectionExited(Domain, this);
                    FleckLog.Info(string.Format("Connection {0}:{1} Exited Domain {2}.", info.ClientIpAddress, info.ClientPort, Domain.Path));

                    if (Domain.connections.Count == 0) Domain.Close();
                }
            }

            Server.connections.Remove(this);

            FleckLog.Info(string.Format("Connection {0}:{1} Closed.", info.ClientIpAddress, info.ClientPort));
        }

        public void Send(string message) => connection.Send(message);
        public void Send(byte[] message) => connection.Send(message);
        public void Close() => connection.Close();
    }
}