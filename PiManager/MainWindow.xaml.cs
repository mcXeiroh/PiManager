﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace PiManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddPi_Click(object sender, RoutedEventArgs e)
        {
            PiListView.Items.Add(new PiData {
                IPaddress = newIP_TB.Text,
                Status = PiData.connectionStatus.offline.ToString(),
                Ping = "-- ms"
            });

            PiList.ip.Add(PiList.ID, newIP_TB.Text);
            PiList.status.Add(PiList.ID, PiData.connectionStatus.offline);
            PiList.ID++;
        }

        public void PingAll()
        {
            for (int i = 1; i < PiList.ID; i++)
            {
                try
                {
                    string ipString = PiList.ip[i];

                    IPAddress ip = IPAddress.Parse(ipString);
                    PingOptions pingOptions = new PingOptions(128, true);
                    Ping ping = new Ping();
                    byte[] buffer = new byte[32];

                    try
                    {
                        PingReply pingReply = ping.Send(ip, 1000, buffer, pingOptions);
                        if (!(pingReply == null))
                        {
                            switch (pingReply.Status)
                            {
                                case IPStatus.Success:
                                    PiListView.Items.RemoveAt(i - 1);
                                    PiListView.Items.Insert(i - 1, new PiData
                                    {
                                        IPaddress = ipString,
                                        Status = PiData.connectionStatus.online.ToString(),
                                        Ping = pingReply.RoundtripTime + " ms"
                                    });
                                    PiList.status.Add(i, PiData.connectionStatus.online);
                                    break;
                                case IPStatus.TimedOut:
                                    PiListView.Items.RemoveAt(i - 1);
                                    PiListView.Items.Insert(i - 1, new PiData
                                    {
                                        IPaddress = ipString,
                                        Status = PiData.connectionStatus.timeOut.ToString(),
                                        Ping = "-- ms"
                                    });
                                    break;
                                default:
                                    PiListView.Items.RemoveAt(i - 1);
                                    PiListView.Items.Insert(i - 1, new PiData
                                    {
                                        IPaddress = ipString,
                                        Status = PiData.connectionStatus.notFound.ToString(),
                                        Ping = "-- ms"
                                    });
                                    break;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ping to:" + ipString + " - failed", "Ping failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (PingException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    catch (SocketException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("unhandled error");
                }
            }
        }

        private void Ping_Click(object sender, RoutedEventArgs e)
        {
            PingAll();
        }
    }


    public class PiData
    {
        public enum connectionStatus { offline, timeOut, notFound, online, connected };

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
    }
}