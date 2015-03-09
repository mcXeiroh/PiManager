using System;
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
using System.Windows.Shapes;
using System.Threading;

namespace PiManager
{
    /// <summary>
    /// Interaktionslogik für Model.xaml
    /// </summary>
    public partial class Model : Window
    {
        static bool[] inputState = new bool[8];
        static bool[] outputState = new bool[8];

        static Thread modelThread = new Thread(new ThreadStart(StartProgram));

        public Model()
        {
            InitializeComponent();
        }

        public static void StartProgram()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 1; j <= 8; j++)
                {
                    ClearAllDigital();
                    SetDigitalChannel(j);
                    Thread.Sleep(500);
                }
                for (int j = 8; j >= 1; j--)
                {
                    ClearAllDigital();
                    SetDigitalChannel(j);
                    Thread.Sleep(500);
                }
            }
        }

        public static bool ReadDigitalChannel(int channel)
        {
            return inputState[channel - 1];
        }

        public static void ClearAllDigital()
        {
            for (int i = 1; i <= 8; i++)
            {
                ClearDigitalChannel(i);
            }
        }

        public static void ClearDigitalChannel(int channel)
        {
            Pin channelPin = new Pin();
            channelPin.IO = Pin.GPIO.o;
            channelPin.No = channel;
            channelPin.State = false;

            ChangeOutput(channelPin);
        }

        public static void SetDigitalChannel(int channel)
        {
            Pin channelPin = new Pin();
            channelPin.IO = Pin.GPIO.o;
            channelPin.No = channel;
            channelPin.State = true;

            ChangeOutput(channelPin);
        }

        private void Input_Click(object sender, RoutedEventArgs e)
        {
            CheckBox input = (CheckBox) e.Source;
            string name = input.Content.ToString();
            
            ChangeInput(new Pin
            {
                IO = Pin.GPIO.i,
                No = Convert.ToInt32(name),
                State = Convert.ToBoolean(input.IsChecked)
            });
        }

        public static void ChangeInput(Pin pin)
        {
            inputState[pin.No - 1] = pin.State;

            Model model = new Model();
            switch (pin.No)
            {
                case 1:
                    model.input1.IsChecked = pin.State;
                    break;
                case 2:
                    model.input2.IsChecked = pin.State;
                    break;
                case 3:
                    model.input3.IsChecked = pin.State;
                    break;
                case 4:
                    model.input4.IsChecked = pin.State;
                    break;
                case 5:
                    model.input5.IsChecked = pin.State;
                    break;
                case 6:
                    model.input6.IsChecked = pin.State;
                    break;
                case 7:
                    model.input7.IsChecked = pin.State;
                    break;
                case 8:
                    model.input8.IsChecked = pin.State;
                    break;
            }
        }

        public static void ChangeOutput(Pin pin)
        {
            outputState[pin.No - 1] = pin.State;
            Model model = new Model();
            
            switch (pin.No)
            {
                case 1:
                    model.outputRadio1.Dispatcher.BeginInvoke((Action)(() => model.outputRadio1.IsChecked = pin.State));
                    break;
                case 2:
                    model.outputRadio2.Dispatcher.BeginInvoke((Action)(() => model.outputRadio2.IsChecked = pin.State));
                    break;
                case 3:
                    model.outputRadio3.Dispatcher.BeginInvoke((Action)(() => model.outputRadio3.IsChecked = pin.State));
                    break;
                case 4:
                    model.outputRadio4.Dispatcher.BeginInvoke((Action)(() => model.outputRadio4.IsChecked = pin.State));
                    break;
                case 5:
                    model.outputRadio5.Dispatcher.BeginInvoke((Action)(() => model.outputRadio5.IsChecked = pin.State));
                    break;
                case 6:
                    model.outputRadio6.Dispatcher.BeginInvoke((Action)(() => model.outputRadio6.IsChecked = pin.State));
                    break;
                case 7:
                    model.outputRadio7.Dispatcher.BeginInvoke((Action)(() => model.outputRadio7.IsChecked = pin.State));
                    break;
                case 8:
                    model.outputRadio8.Dispatcher.BeginInvoke((Action) (() => model.outputRadio8.IsChecked = pin.State));
                    break;
            }

            ConnectionHandler.BroadcastPin(pin);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            modelThread.SetApartmentState(ApartmentState.STA);
            modelThread.Start();
        }
    }
}
