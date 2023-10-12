using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        private static readonly string[] Words = new string[]
            { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine","Giá trị không đúng" };
        static async Task Main(string[] args)
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.Unicode;

            try
            {
                IPAddress ipAddress = IPAddress.Loopback;
                var serverPort = 7777;

                Console.WriteLine($"Server nghe ở : ${ipAddress}:{serverPort}");

                TcpListener listener = new TcpListener(ipAddress, serverPort);
                Console.WriteLine("Đang đợi kết nối từ client.....");
                listener.Start();
                while (true)
                {
                    await Task.Run(() => HandleClient(listener));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi..... " + e);
            }

            Console.ReadLine();
        }
        static async void HandleClient(TcpListener tcpListener)
        {
            Socket clientSocket = await tcpListener.AcceptSocketAsync();
            try
            {

                Console.WriteLine("Kết nối từ " + clientSocket.RemoteEndPoint);
                var unicodeEncoding = new UnicodeEncoding();
                Console.WriteLine("Đợi dữ liệu từ client... ");
                while (true)
                {
                    byte[] binDataIn = new byte[255];
                    int k = clientSocket.Receive(binDataIn);

                    if (k == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(
                            $"Kết nối từ {clientSocket.RemoteEndPoint} đã đóng");
                        clientSocket.Close();
                        Console.ResetColor();
                        break;
                    }

                    string msg = unicodeEncoding.GetString(binDataIn, 0, k);
                    bool isNumber = int.TryParse(msg, out var number);
                    string returnMessage;
                    if (isNumber)
                    {
                        var wordIndex = number >= 0 && number <= 9 ? number : Words.Length - 1;
                        returnMessage = Words[wordIndex];
                        Console.WriteLine($"Đã gửi dữ liệu cho : {clientSocket.RemoteEndPoint}");
                    }
                    else
                    {
                        returnMessage = "Dữ liệu đầu vào phải là số, vui lòng thử lại";
                    }

                    clientSocket.Send(unicodeEncoding.GetBytes(returnMessage));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi xử lý client: " + e);
            }
            finally
            {
                clientSocket.Close();
                //tcpListener.Stop();
            }
        }
    }
}
