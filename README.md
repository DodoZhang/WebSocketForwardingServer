# WSFS (Web Socket Forwarding Server)

WSFS is a lightweight Web Socket Forwarding Server framework based on [Fleck](github.com/statianzo/Fleck). It is an easy-to-embed framework that supports multi-channel communication and custom login checks.

WSFS 是一个基于 [Fleck](github.com/statianzo/Fleck) 的轻量化的 Web Socket 转发服务器框架。它是一个易于嵌入且支持多通道通讯和自定义登录校验的框架。

[中文介绍](README.zh_cn.md)

This repository contains a console application, WSFSConsoleApp, as an example, in addition to the core forwarding server framework, WSFS.

## Specialized Terms

We call these connections, which use the same URL, in the same domain. Messages sent from one link to the server are forwarded to all other connections in the same domain as the connection.  
When a link connects to the server, it joins the domain whose URL corresponds to it; similarly, when a link disconnects from the server, it leaves the domain to which it belongs.  
When a connection joins a domain where there are no other connections, we call the connection created that domain.

## WSFS Usage Examples

The following example creates a forwarding server on port 8080 that listens on all addresses.

```cs
WSFSServer server = new("ws://0.0.0.0:8080");
server.Start();
```

In this case, the server forwards messages from the connection to all connections in the same domain, i.e., all connections that use the same URL.  
For example, if connections 1, 2, and 3 used `ws://localhost:8080/xxx` to connect to the server, and connections 4 and 5 used `ws://localhost:8080/yyyy` to connect to the server.  
If connection 1 sends a message to the server, the server forwards it to connections 2 and 3; similarly, if connection 4 sends a message to the server, the server forwards it to connection 5.

### Secure Connection (wss://)

We can enable secure connections by simply replacing `ws` with `wss` in the protocol and providing an x509 certificate containing both the public and private keys.

```cs
WSFSServer server = new("wss://0.0.0.0:8080", new X509Certificate2("MyCert.pfx"));
server.Start();
```

### Customizing Login Validation And Exit Events

We can customize the login validation and exit events by inheriting and overriding the `OnConnectionJoining` and `OnConnectionExited` methods in `WSFSBehaviour`.

In the following example, we would send message `"Welcome!"` to a connection when it joins a domain, message `"... Joined the domain."` to other connections in the domain that the connection is about to join, and message `"... Exited the domain."` to other connections in the domain when the connection leaves the domain.

```cs
internal class MyBehaviour : WSFSBehaviour
{
    public override bool OnConnectionJoining(WSFSDomain domain, WSFSConnection connection)
    {
        connection.Send("Welcome!");
        foreach (WSFSConnection c in domain.Connections)
            c.Send(string.Format("{0}:{1} Joined the domain.", connection.ClientIPAddress, connection.ClientPort));
        return true;
    }

    public override void OnConnectionExited(WSFSDomain domain, WSFSConnection connection)
    {
        foreach (WSFSConnection c in domain.Connections)
            c.Send(string.Format("{0}:{1} Exited the domain.", connection.ClientIPAddress, connection.ClientPort));
    }
}
```

```cs
WSFSServer server = new("ws://0.0.0.0:8080");
server.Behaviour = new MyBehaviour();
server.Start();
```

Connection joining to a domain can be denied by returning `false` in the overloaded `OnConnectionJoining` method.

In the following example, we will only allow local (`127.0.0.1`) connections to create domains (i.e. join a domain with no other connections).

```cs
internal class MyBehaviour : WSFSBehaviour
{
    public override bool OnConnectionJoining(WSFSDomain domain, WSFSConnection connection)
    {
        return domain.Connections.Count != 0 || connection.ClientIPAddress == "127.0.0.1";
    }
}
```

```cs
WSFSServer server = new("ws://0.0.0.0:8080");
server.Behaviour = new MyBehaviour();
server.Start();
```

## WSFSConsoleApp Usage Example

The WSFS Console App provides a password-enabled forwarding server. Connections can create or join domains using a URL such as `ws://127.0.0.1:8080/test_domain?password=<password>`.  
When creating a domain, the contents of the `password` parameter are used as the password for the domain, and when other connections try to join the domain, they need to provide the same `password` parameter as when the domain was created in order to join the domain.

The console application can be started using a command such as the following:

```bat
wsfs ws://0.0.0.0:8080
```

Or

```bat
wsfs wss://0.0.0.0:8080 MyCert.pfx
```