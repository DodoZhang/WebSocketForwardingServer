using Fleck;

namespace WSFS
{
    public class WSFSDomain
    {
        public WSFSServer Server { get; private set; }
        public string Path { get; private set; }

        public IReadOnlySet<WSFSConnection> Connections => connections;
        internal HashSet<WSFSConnection> connections = new();

        internal WSFSDomain(WSFSServer server, string path)
        {
            Server = server;
            Path = path;

            Open();
        }

        private void Open()
        {
            FleckLog.Info(string.Format("Domain {0} Opened.", Path));
            Server.domains.Add(Path, this);
        }

        internal void Close()
        {
            FleckLog.Info(string.Format("Domain {0} Closed.", Path));
            Server.domains.Remove(Path);
        }
    }
}