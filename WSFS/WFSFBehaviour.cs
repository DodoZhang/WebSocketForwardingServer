using Fleck;

namespace WSFS
{
    public class WSFSBehaviour
    {
        /// <summary>
        /// Called when a connection is trying to join a domain.<br/>
        /// Return <c>true</c> to allow, return <c>false</c> to deny.
        /// </summary>
        /// <param name="domain">
        /// The domain which the connection is trying to join.
        /// </param>
        /// <param name="connection">
        /// The connection which is trying to join the domain.
        /// </param>
        /// <returns>Whether to allow the connection to join the domain.</returns>
        public virtual bool OnConnectionJoining(WSFSDomain domain, WSFSConnection connection)
        {
            return true;
        }

        /// <summary>
        /// Called after a connection exited a domain.
        /// </summary>
        /// <param name="domain">
        /// The domain which the connection exits.
        /// </param>
        /// <param name="connection">
        /// The connection which exit the domain.
        /// </param>
        public virtual void OnConnectionExited(WSFSDomain domain, WSFSConnection connection) { }
    }
}