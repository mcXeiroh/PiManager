using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace PiManager
{
    class ConnectionHandler
    {
        public const int Port = 11000;
        public const int BuffSize = 100;
        public static MainWindow main = new MainWindow();

        public static void ConnectAll()
        {
            
            for (int i = 1; i < PiList.ID; i++)
            {
                PiList.clientThread.Add(i, StartThread(i));
            }
        }

        public static Thread StartThread(int i)
        {
            var thread = new Thread(() => Connect(i));
            thread.Start();
            return thread;
        }
        
        public static void Connect(int ID)
        {
            try
            { 
                string ipString = PiList.ip[ID];

                PiList.tcpClient.Add(ID, new TcpClient()); //Create new TcpClient
                PiList.tcpClient[ID].Connect(ipString, Port); //Connect to listener

                NetworkStream stream = PiList.tcpClient[ID].GetStream();

                UTF8Encoding utEn = new UTF8Encoding();
                byte[] buffer = utEn.GetBytes(ID.ToString());

                stream.Write(buffer, 0, buffer.Length);

                byte[] readBuffer = new byte[BuffSize];
                int k = stream.Read(readBuffer, 0, BuffSize);

                string received = utEn.GetString(readBuffer, 0, k);

                
                if (received != "valid")
                {
                    main.UpdateView(ID, PiData.connectionStatus.error);
                } 
                else
                {
                    main.UpdateView(ID, PiData.connectionStatus.connected);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                main.UpdateView(ID, PiData.connectionStatus.error);
            }
        }
    }
}
