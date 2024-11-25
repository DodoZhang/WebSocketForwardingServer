using Fleck;

namespace WSFS
{
    public class WSFSBehaviour
    {
        /// <summary>
        /// Called when a connection is trying to join a domain.<br/>
        /// Return <c>true</c> to allow, return <c>false</c> to deny.
        /// </summary>
        /// <param name="domain">The domain which the connection is trying to join.</param>
        /// <param name="connection">The connection who is trying to join the domain.</param>
        /// <returns>Whether to allow the connection to join the domain.</returns>
        public virtual bool OnConnectionJoining(WSFSDomain domain, WSFSConnection connection)
        {
            return true;
        }

        /// <summary>
        /// Called before a message be forwarded.<br/>
        /// Return whether you want to forward the message.
        /// </summary>
        /// <param name="domain">The domain which recieved the message.</param>
        /// <param name="connection">The connection who sent the message.</param>
        /// <param name="message">The message the server recieved.</param>
        /// <returns>Whether to forward the message or not.</returns>
        public virtual bool OnForwardingMessage(WSFSDomain domain, WSFSConnection connection, string message)
        {
            return true;
        }

        /// <summary>
        /// Called before a binary message be forwarded.<br/>
        /// Return whether you want to forward the binary message.
        /// </summary>
        /// <param name="domain">The domain which recieved the message.</param>
        /// <param name="connection">The connection who sent the message.</param>
        /// <param name="message">The binary message the server recieved.</param>
        /// <returns>Whether to forward the binary message or not.</returns>
        public virtual bool OnForwardingMessage(WSFSDomain domain, WSFSConnection connection, byte[] message)
        {
            return true;
        }

        /// <summary>
        /// Called after a connection exited a domain.
        /// </summary>
        /// <param name="domain">The domain which the connection exits.</param>
        /// <param name="connection">The connection who exit the domain.</param>
        public virtual void OnConnectionExited(WSFSDomain domain, WSFSConnection connection) { }
    }
}