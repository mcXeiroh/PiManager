﻿using System;
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

    public class PiData
    {
        public enum connectionStatus { offline, timeOut, notFound, online, connected, error };

        public string IPaddress { get; set; }
        public string Status { get; set; }
        public string Ping { get; set; }
    }

    /// <summary>
    /// This class is to manage all entered and connected pi's.
    /// </summary>
    public class PiList
    {
        public static int ID = 1;
        public static Dictionary<int, string> ip = new Dictionary<int, string>();
        public static Dictionary<int, PiData.connectionStatus> status = new Dictionary<int, PiData.connectionStatus>();
        public static Dictionary<int, TcpClient> tcpClient = new Dictionary<int, TcpClient>();
        public static Dictionary<int, Thread> clientThread = new Dictionary<int, Thread>();
        public static Dictionary<int, NetworkStream> stream = new Dictionary<int, NetworkStream>();
    }
}