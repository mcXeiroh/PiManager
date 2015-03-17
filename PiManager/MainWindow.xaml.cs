using System;
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
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Diagnostics;

namespace PiManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CloseApp);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            statusEllipse.Fill = Brushes.Red;
        }

        static void CurrentDomain_UnhandledException (object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;

                MessageBox.Show("Whoops! An error occurred: " + ex.Message, "Whoops! An error occurred!", MessageBoxButton.OK);
            }
            finally
            {
                CloseApp(null, EventArgs.Empty);
            }
        }

        /**
         * Adds a pi with the ip from the textbox to all variables
         * 
         * @param sender
         * @param EventArgs
         */
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

        /**
         * Not used method. Actually it should show in the list view the state of the connection to a pi, e.g. connected.
         */
        public void UpdateView()
        {
            /*
            foreach (var item in PiList.status)
            {
                PiListView.Items.RemoveAt(item.Key - 1);
                PiListView.Items.Insert(item.Key - 1, new PiData
                {
                    IPaddress = PiList.ip[item.Key],
                    Status = item.Value.ToString(),
                    Ping = "-- ms"
                });
            } */
        }

        /**
         * Pings all pi's which are in the listview and updates their ping in the list.
         */
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

                                    if (!PiList.status.ContainsKey(i)) PiList.status.Add(i, PiData.connectionStatus.online);
                                    else PiList.status[i] = PiData.connectionStatus.online;

                                    break;

                                case IPStatus.TimedOut:
                                    PiListView.Items.RemoveAt(i - 1);
                                    PiListView.Items.Insert(i - 1, new PiData
                                    {
                                        IPaddress = ipString,
                                        Status = PiData.connectionStatus.timeOut.ToString(),
                                        Ping = "-- ms"
                                    });

                                    if (!PiList.status.ContainsKey(i)) PiList.status.Add(i, PiData.connectionStatus.timeOut);
                                    else PiList.status[i] = PiData.connectionStatus.timeOut;

                                    break;

                                default:
                                    PiListView.Items.RemoveAt(i - 1);
                                    PiListView.Items.Insert(i - 1, new PiData
                                    {
                                        IPaddress = ipString,
                                        Status = PiData.connectionStatus.notFound.ToString(),
                                        Ping = "-- ms"
                                    });

                                    if (!PiList.status.ContainsKey(i)) PiList.status.Add(i, PiData.connectionStatus.notFound);
                                    else PiList.status[i] = PiData.connectionStatus.notFound;

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
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        /**
         * Handles the ping button
         * 
         * @param sender
         * @param EventArgs
         */
        private void Ping_Click(object sender, RoutedEventArgs e)
        {
            PingAll();
        }

        /**
         * Handles the connect button. Calls the ConnectionHandler, ConnectAll.
         * 
         * @param sender
         * @param EventArgs
         */
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (PiList.ID > 1)
            {
                statusEllipse.Fill = Brushes.Orange;
                addPi_btn.IsEnabled = false;
                connect_btn.IsEnabled = false;

                ConnectionHandler.ConnectAll();
                statusEllipse.Fill = Brushes.Lime;
            }
        }

        /**
         * Handles the clearList button. Remove all entries out of the list view.
         * 
         * @param sender
         * @param EventArgs
         */
        private void ClearList_Click(object sender, RoutedEventArgs e)
        {
            if (!connect_btn.IsEnabled)
            {
                foreach (var item in PiList.clientThread)
                {
                    item.Value.Abort();
                }
                foreach (var item in PiList.tcpClient)
                {
                    item.Value.Close();
                }

                statusEllipse.Fill = Brushes.Red;
                addPi_btn.IsEnabled = true;
                connect_btn.IsEnabled = true;
            }

            PiList.ID = 1;
            PiList.ip.Clear();
            PiList.status.Clear();
            PiList.tcpClient.Clear();
            PiList.clientThread.Clear();

            PiListView.Items.Clear();
        }

        /**
         * AppDomain Event. Calls, when the application closes.
         * 
         * @param sender
         * @param EventArgs
         */
        public static void CloseApp(object sender, EventArgs e)
        {
            Process mainProcess = Process.GetCurrentProcess();
            mainProcess.Kill();
        }

        /**
         * Opens the model view. Shows it in a dialog to avoid bugs.
         * 
         * @param sender
         * @param EventArgs
         */
        private void StartPrg_Click(object sender, RoutedEventArgs e)
        {
            Model modelForm = new Model();
            modelForm.ShowDialog();
        }
    }
}
