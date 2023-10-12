using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.Unicode;
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPAddress hostAddress = IPAddress.Loopback;
                IPEndPoint hostEndPoint = new IPEndPoint(hostAddress, 7777);

                Console.WriteLine("Đang kết nối.....");
                socket.Connect(hostEndPoint);
                if (socket.Connected) Console.WriteLine("Kết nối thành công");

                Console.WriteLine("Dữ liệu gửi đi :   ");
                var unicodeEncoding = new UnicodeEncoding();
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("-> Client: ");
                    string str = Console.ReadLine();

                    if (str != null && str.ToLower() == "quit")
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Đã đóng kết nối tới {hostEndPoint.Address}:{hostEndPoint.Port}");
                        break;
                    }

                    if (str != null)
                    {
                        byte[] binDataOut = unicodeEncoding.GetBytes(str);
                        socket.Send(binDataOut, 0, binDataOut.Length, SocketFlags.None);
                    }

                    byte[] binDataIn = new byte[255];
                    int k = socket.Receive(binDataIn, 0, 255, SocketFlags.None);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    string msg = unicodeEncoding.GetString(binDataIn, 0, k);
                    Console.WriteLine("-> Server: " + msg);
                    Console.ResetColor();
                }

                socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi..... " + e.StackTrace);
            }
            Console.ReadLine();
        }
    }
}
