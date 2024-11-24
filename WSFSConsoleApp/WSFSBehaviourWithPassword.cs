namespace WSFS.ConsoleApp
{
    internal class WSFSBehaviourWithPassword : WSFSBehaviour
    {
        private Dictionary<WSFSDomain, string> passwords = new();

        public override bool OnConnectionJoining(WSFSDomain domain, WSFSConnection connection)
        {
            if (domain.Connections.Count == 0)
            {
                // Set the password of the domain if the connection is the first one who joins the domain.
                if (connection.Parameters.TryGetValue("password", out string? password))
                    passwords.Add(domain, password);
                return true;
            }
            else
            {
                // Check the password if the connection is not the first one who joins the domain.
                if (!passwords.TryGetValue(domain, out string? currectPassword)) return true;
                if (!connection.Parameters.TryGetValue("password", out string? password)) return false;
                return password == currectPassword;
            }
        }

        public override void OnConnectionExited(WSFSDomain domain, WSFSConnection connection)
        {
            if (domain.Connections.Count == 0)
            {
                // Reset the password of the domain if the connection is the last one who exits the domain.
                if (passwords.ContainsKey(domain)) passwords.Remove(domain);
            }
        }
    }
}
