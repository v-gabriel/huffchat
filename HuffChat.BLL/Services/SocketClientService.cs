using HuffChat.BLL.Enums;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HuffChat.BLL.Services
{
    public class SocketClientService
    {
        Socket? client = null;
        Thread? thread = null;

        public async Task Start(string[] args)
        {
            Console.WriteLine($"\nC$$ Socket Client initializing...\n");
            try
            {
                var displayName = $"anonymous{Guid.NewGuid().ToString().Substring(0, 5)}";
                var hostName = String.Empty;

                if (args.Length == 0)
                {
                    Console.WriteLine(
                        $"C$ No parameters provided, using host machine.");

                    hostName = Dns.GetHostName();
                }
                if (args.Length == 2)
                {
                    var ipEndpoint = args[0];
                    var port = args[1];
                    hostName = $"{ipEndpoint}:{port}";
                    Console.WriteLine($"C$ Connecting to... {ipEndpoint} {port} as {displayName}");
                }
                if (args.Length == 3)
                {
                    displayName = args[2];
                    Console.WriteLine($"C$ Display name: {displayName}");
                }

                IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(hostName);
                IPAddress ipAddress = ipHostInfo.AddressList[0];

                IPEndPoint ipEndPoint = new(ipAddress, 11_000);

                client = new(
                    ipEndPoint.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp
                );

                await client.ConnectAsync(ipEndPoint);
                thread = new Thread(async () =>
                {
                    await ReplyListener();
                });
                thread.Start();

                Console.WriteLine($"C$ Connection established, you can send messages now.");
                Console.WriteLine("\n");
                while (true)
                {
                    string? message = Console.ReadLine() ?? String.Empty;

                    message = $"[{displayName}] {message}";
                    message += SocketResponse.END_OF_MESSAGE; ;

                    var messageBytes = Encoding.UTF8.GetBytes(message);
                    _ = await client.SendAsync(messageBytes, SocketFlags.None);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                client?.Shutdown(SocketShutdown.Send);
            }
        }

        private async Task ReplyListener()
        {
            while(true)
            {
                if (client != null)
                {
                    var buffer = new byte[1_024];
                    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                    var response = Encoding.UTF8.GetString(buffer, 0, received);
                    if (response.IndexOf(SocketResponse.END_OF_MESSAGE) > -1)
                    {
                        var reply = $"$: {response.Replace(SocketResponse.END_OF_MESSAGE, "")}";
                        Console.WriteLine(reply);
                    }
                }
            }
        }
    }
}
