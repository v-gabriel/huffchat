using HuffChat.BLL.Services;

namespace HuffChat.Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var service = new SocketClientService();
            await service.Start(args);
        }
    }
}