using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Event_timer
{
    internal class UDP
    {
        //이벤트 발생 delegate
        public delegate void Data_Recieved(string data);
        public event Data_Recieved data_recieved;

        private Thread udp_recv_thread;//broadcast 되는 data 받기위한 Thread'
        private static string ip_address = "";
        private int PORT = 2100;
        UdpClient udp = new UdpClient();
        bool ExitFlag = false;

        public UDP()
        {
            ExitFlag = true;
            udp_recv_thread = new Thread(async()=> await Run());
            udp_recv_thread.Start();
            udp.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));
        }

        private async Task Run()
        {
            while (ExitFlag)
            {
                UdpReceiveResult receiveResult;
                try
                {
                    receiveResult = await udp.ReceiveAsync();
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("UDP 데이터 수신 오류: " + ex.Message);
                    continue;
                }

                string receivedData = Encoding.ASCII.GetString(receiveResult.Buffer);
                data_recieved(receivedData);
            }
        }

        public void SendData(string data, string ipAddress, int port)
        {
            try
            {
                UdpClient udpClient = new UdpClient();
                byte[] bytes = Encoding.ASCII.GetBytes(data);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                udpClient.Send(bytes, bytes.Length, endPoint);
                udpClient.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending UDP data: " + ex.Message);
            }
        }

        public void Kill_Thread_UDP()
        {
            ExitFlag = false;
        }
    }
}
