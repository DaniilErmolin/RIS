using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Chat.Core;
using Chat.Models;
using Chat.Services;
using NUnit.Framework;

namespace Chat.Tests.Core;

[TestFixture]
[TestOf(typeof(UdpChatServer))]
public class UdpChatServerTest
{

    [Test]
    public void ServerCreationTest()
    {
        int testPort = 12345;

        UdpChatServer server = new UdpChatServer(testPort);
        server.Start();

        Assert.That(server, Is.Not.Null, "UdpServer instance should not be null.");
    }
}