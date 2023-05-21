using HuffChat.BLL.Services;
using System.Net;

namespace HuffChat.Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var service = new SocketServerService(Dns.GetHostName());
            await service.Start();
        }
    }
}