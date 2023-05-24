using HuffChat.BLL.Enums;
using HuffChat.BLL.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HuffChat.BLL.Services
{
    public class SocketServerService
    {
        List<Thread> threads = new List<Thread>();
        List<Socket> handlers = new List<Socket>();

        HuffmanTree huffmanTree = new HuffmanTree();

        IPHostEntry ipHostInfo;
        IPAddress ipAddress;

        public SocketServerService(string hostName)
        {
            ipHostInfo = Dns.GetHostEntry(hostName);
            ipAddress = ipHostInfo.AddressList[0];

            huffmanTree.Build(AppConstants.HUFFMAN_DEFAULT_BUILD_INPUT);
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
                Console.WriteLine($"S$ Started Socket Server\n$ IP endpoint: {ipEndpoint}");
#else
                Console.WriteLine($"S$ Started Socket Server.");
#endif
                Console.WriteLine("\n");

                var mainThread = new Thread(async () =>
                {
                    await AcceptNextConnection(listener);
                });
                mainThread.Start();
                while (true) { }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task AcceptNextConnection(Socket listener)
        {
            var isFirstConnect = true;
            var handler = await listener.AcceptAsync();

            var thread = new Thread(async () =>
            {
                await AcceptNextConnection(listener);
            });
            thread.Start();
            threads.Add(thread);
            handlers.Add(handler);
            
            try
            {
                while (true)
                {
                    if (!handler.Connected)
                    {
                        break;
                    }
                    if (isFirstConnect)
                    {
                        var encodingInput = AppConstants.HUFFMAN_DEFAULT_BUILD_INPUT;
                        var echoBytes = Encoding.UTF8.GetBytes(encodingInput);
                        foreach (var h in handlers)
                        {
                            await h.SendAsync(echoBytes, SocketFlags.None);
                        }
                        isFirstConnect = false;
                    }

                    var buffer = new byte[1_024];
                    var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                    var response = Encoding.UTF8.GetString(buffer, 0, received);

                    if (response.IndexOf(SocketResponse.END_OF_MESSAGE) > -1)
                    {
                        var bitMessage = ParserHelper.ParseBitArrayFromString(response);
                        var decoded = huffmanTree.Decode(bitMessage).ToString() ?? "";

                        var reply = $"$: {decoded.Replace(SocketResponse.END_OF_MESSAGE, "")}";
                        // TODO: Fix parsing (decodedMessage has an extra char at the end)
                        reply = reply.Substring(0, reply.Length - 1);
                        Console.WriteLine($"{reply}");

                        var echoBytes = Encoding.UTF8.GetBytes(response);
                        foreach (var h in handlers.Where(x => x.Connected))
                        {
                            await h.SendAsync(echoBytes, SocketFlags.None);
                        }
                    }
                }
                this.cleanHandler(handler);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nS$ Error: {ex.Message}\n");
            }
            finally
            {
                this.cleanHandler(handler);
            }
        }

        private void cleanHandler(Socket handler)
        {
            this.handlers.Remove(handler);
            handler.Close();
        }
    }
}
