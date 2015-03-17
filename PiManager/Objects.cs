using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace PiManager
{
    class Objects
    {
    }

    /*! Object class for handling the pi status*/
    public class PiData
    {
        public enum connectionStatus { offline, timeOut, notFound, online, connected, error };

        public string IPaddress { get; set; }
        public string Status { get; set; }
        public string Ping { get; set; }
    }

    /*! Central class to store all information about all pi's */
    public class PiList
    {
        public static int ID = 1;
        public static Dictionary<int, string> ip = new Dictionary<int, string>();
        public static Dictionary<int, PiData.connectionStatus> status = new Dictionary<int, PiData.connectionStatus>();
        public static Dictionary<int, TcpClient> tcpClient = new Dictionary<int, TcpClient>();
        public static Dictionary<int, Thread> clientThread = new Dictionary<int, Thread>();
        public static Dictionary<int, NetworkStream> stream = new Dictionary<int, NetworkStream>();
    }

    /*! Object class for handling pin's */
    public class Pin
    {
        public enum GPIO { i, o };

        public GPIO IO { get; set; }
        public int No { get; set; } //Number
        public bool State { get; set; }
    }
}
