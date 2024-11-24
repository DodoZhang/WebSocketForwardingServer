# WSFS (Web Socket Forwarding Server)

WSFS 是一个基于 [Fleck](github.com/statianzo/Fleck) 的轻量化的 Web Socket 转发服务器框架。它是一个易于嵌入且支持多通道通讯和自定义登录校验的框架。

WSFS is a lightweight Web Socket Forwarding Server framework based on [Fleck](github.com/statianzo/Fleck). It is an easy-to-embed framework that supports multi-channel communication and custom login checks.

[English](README.md)

本仓库中除了转发服务器核心框架 WSFS 外，还包含了一个作为示例的控制台应用 WSFSConsoleApp。

## 专用名词

我们称这些使用同样 URL 的连接处于同一个域。一个链接向服务器发送的消息会被转发给所有其他与连接同一个域中连接。  
当一个连接上服务器时，这个连接就会加入其 URL 对应的域；同理当一个连接断开与服务器的链接时，它就会离开其所属的域。  
当连接加入一个没有其他连接的域时，我们称这个连接创建了该域。

## WSFS 使用实例

下述实例可以在 8080 端口创建一个监听所有地址的转发服务器。

```cs
WSFSServer server = new("ws://0.0.0.0:8080");
server.Start();
```

此时，服务器会将连接发出的消息转发给同一个域中，即所有使用同样 URL 的连接。  
例如若连接 1、2、3 使用 `ws://localhost:8080/xxx` 连接了服务器，连接 4、5 使用 `ws://localhost:8080/yyy` 连接了服务器。  
此时若连接 1 向服务器发送了一段消息，服务器会将这段消息转发给连接 2 和 3；同理若连接 4 向服务器发送了一段消息，服务器只会将这段消息转发给连接 5。

### 安全连接 (wss://)

我们只需要将协议中的 `ws` 替换 `wss` 并提供一个包含公钥和私钥的 x509 证书，便可以启用安全连接。

```cs
WSFSServer server = new("wss://0.0.0.0:8080", new X509Certificate2("MyCert.pfx"));
server.Start();
```

### 自定义登录校验和退出事件

我们可以通过继承并重载 `WSFSBehaviour` 中的 `OnConnectionJoining` 和 `OnConnectionExited` 方法来自定义登录校验和退出事件。

在下述示例中，我们会在连接加入域时向这个连接发送 `"Welcome!"` 信息，并向连接即将加入的域中的其他连接发送 `"... Joined the domain."` 信息；在连接离开域向域中的其他连接发送 `"... Exited the domain."` 信息。

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

通过在重载的 `OnConnectionJoining` 方法中返回 `false` 可以拒绝连接加入域。

在下述示例中，我们只允许本机（`127.0.0.1`）的连接创建域（即加入一个没有其他连接的域）。

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

## WSFSConsoleApp 使用实例

WSFS Console App 提供了一个支持密码的转发服务器。连接可以使用形如 `ws://127.0.0.1:8080/test_domain?password=<password>` 的 URL 创建或加入域。  
在创建域时，我们会将参数 `password` 的内容作为域的密码，当其他连接尝试加入域时，需要提供域创建时相同的参数 `password` 才能加入域。

可以使用形如以下内容的指令来启动该控制台应用：

```bat
wsfs ws://0.0.0.0:8080
```

或

```bat
wsfs wss://0.0.0.0:8080 MyCert.pfx
```