using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
        public const int Port = 11000; /**< Port to connect to the Pi's */
        public const int BuffSize = 100; /**< Socket buffer size */
        public static MainWindow main = new MainWindow();
        public static Model model = new Model();

        /**
         * Connect all Pi's which were added into the PiListView
         */
        public static void ConnectAll()
        {
            for (int i = 1; i < PiList.ID; i++)
            {
                PiList.clientThread.Add(i, StartThread(i));
            }
        }

        /**
         * Start method for every connection thread
         * 
         * @param Start connection with the Pi's ID
         * @return Thread which has been started
         */
        public static Thread StartThread(int i)
        {
            var thread = new Thread(() => Connect(i));
            thread.IsBackground = true;
            thread.Start();
            return thread;
        }
        
        /**
         * Main connection method
         * 
         * @param Pi's ID
         */
        public static void Connect(int ID)
        {
            try
            { 
                string ipString = PiList.ip[ID];

                PiList.tcpClient.Add(ID, new TcpClient()); //Create new TcpClient
                PiList.tcpClient[ID].Connect(ipString, Port); //Connect to listener

                PiList.stream.Add(ID, PiList.tcpClient[ID].GetStream());

                SendMessage(PiList.stream[ID], ID.ToString());

                string received = ReadMessage(PiList.stream[ID]);

                
                if (received == "valid")
                {
                    PiList.status[ID] = PiData.connectionStatus.connected;
                    main.UpdateView();
                } 
                else
                {
                    PiList.status[ID] = PiData.connectionStatus.error;
                    main.UpdateView();
                }

                WaitForMessage(PiList.stream[ID]);
            }
            catch (Exception ex)
            {
                PiList.status[ID] = PiData.connectionStatus.error;
                main.UpdateView();
                MessageBox.Show(ex.ToString());
            }

        }

        /**
         * Send a message to the NetworkStream of a special Pi
         * 
         * @param NetworkStream to send a message
         * @param Message to send
         */
        public static void SendMessage(NetworkStream stream, string msg)
        {
            try {
                UTF8Encoding utEn = new UTF8Encoding();
                byte[] buffer = utEn.GetBytes(msg);

                stream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /**
         * Read a Message from a special Pi
         * 
         * @param NetworkStream to receive a message
         * @return Received message
         */
        public static string ReadMessage(NetworkStream stream)
        {
            try {
                StreamReader reader = new StreamReader(stream);
                string msg = reader.ReadLineSingleBreak();
                return msg;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        /**
         * Update an input with the message of a network stream
         * 
         * @param NetworkStream to receive input updates
         */
        public static void WaitForMessage(NetworkStream stream)
        {
            while (true)
            {
                Model.ChangeInput(StringInterpreter.ReadMessage(ReadMessage(stream)));
            }
        }

        /**
         * Sends to all Pi's an output update
         * 
         * @param Pin which has to be updated
         */
        public static void BroadcastPin(Pin pin)
        {
            foreach (var item in PiList.stream)
            {
                SendMessage(item.Value, StringInterpreter.BuildMessage(pin));
            }
        }
    }

    public static class StreamReaderExtensions
    {
        public static string ReadLineSingleBreak(this StreamReader reader)
        {
            StringBuilder currentLine = new StringBuilder();
            int i;
            char c;
            while ((i = reader.Read()) >= 0)
            {
                c = (char) i;
                if (c == '\r' || c == '\n') break;
                currentLine.Append(c);
            }
            
            return currentLine.ToString();
        }
    }
}
