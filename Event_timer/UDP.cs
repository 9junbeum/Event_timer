using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Event_timer
{
    internal class UDP
    {
        public delegate void Data_Recieved(string data);//이벤트 발생 delegate
        public event Data_Recieved data_recieved;
        private Thread udp_recv_thread;//broadcast 되는 data 받기위한 Thread
        bool ExitFlag = false;

        UdpClient udp = new UdpClient();
        private static string ip_address = "192.168.100.255";
        private int PORT = 5391;

        //명령어
        public string PDON = "@ONPDRELAY;";
        public string PDOFF = "@OFFPDRELAY;";

        public string Selector_ALL = "@SETALLONF&1000:1;";
        public string Selector_NONE = "@SETALLONF&1000:0;";
        public string Selector_custom = "@SETRIOONF&1000=1:1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0;";

        //Hard Coding 명령어
        public string SELECTOR_ON  = "@SETRIOONF&1000=1:1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0;";
        public string SELECTOR_OFF = "@SETRIOONF&1000=1:0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;";

        public UDP()
        {
            ExitFlag = true;
            udp_recv_thread = new Thread(async()=> await Run());
            udp_recv_thread.Start();
            udp.Client.Bind(new IPEndPoint(IPAddress.Any, PORT)); //모든 IP주소에 대해 수신 대기.
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

        public void SendData(string data, string ipAddress, int port) // 특정 ip주소로 send
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

        public void broadcast(string data)//broadcast
        {
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(data);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip_address), PORT);
                udp.Send(bytes, bytes.Length, endPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error broadcasting UDP data: " + ex.Message);
            }
        }

        public void Kill_Thread_UDP()
        {
            ExitFlag = false;
        }
    }
}
