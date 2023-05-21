using HuffChat.BLL.Enums;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HuffChat.BLL.Services
{
    public class SocketServerService
    {
        List<Thread> threads = new List<Thread>();
        List<Socket> handlers = new List<Socket>();

        IPHostEntry ipHostInfo;
        IPAddress ipAddress;

        public SocketServerService(string hostName)
        {
            ipHostInfo = Dns.GetHostEntry(hostName);
            ipAddress = ipHostInfo.AddressList[0];
        }

        public async Task Start()
        {
            Console.WriteLine($"\nS$$ Socket Server initializing...\n");
            try
            {
                ipHostInfo = await Dns.GetHostEntryAsync(Dns.GetHostName());
                ipAddress = ipHostInfo.AddressList[0];

                IPEndPoint ipEndpoint = new(ipAddress, 11_000);

                using Socket listener = new(
                    ipEndpoint.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp
                );

                listener.Bind(ipEndpoint);
                listener.Listen(100);

                #if DEBUG
                    Console.WriteLine($"S$ Started Socket Server.);
                #else
                    Console.WriteLine($"S$ Started Socket Server\n$ IP endpoint: {ipEndpoint}");
                #endif
                Console.WriteLine("\n");
                await AcceptNextConnection(listener);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task AcceptNextConnection(Socket listener)
        {
            var handler = await listener.AcceptAsync();

            var thread = new Thread(async () =>
            {
                await AcceptNextConnection(listener);
            });
            thread.Start();
            threads.Add(thread);
            handlers.Add(handler);

            while (true)
            {
                var buffer = new byte[1_024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);

                if (response.IndexOf(SocketResponse.END_OF_MESSAGE) > -1)
                {
                    var message = $"{response.Replace(SocketResponse.END_OF_MESSAGE, "")}";
                    Console.WriteLine($"$: {message}");

                    var echoBytes = Encoding.UTF8.GetBytes(message += SocketResponse.END_OF_MESSAGE);

                    foreach(var h in handlers)
                    {
                        await h.SendAsync(echoBytes, 0);
                    }
                }
            }
        }
    }
}
