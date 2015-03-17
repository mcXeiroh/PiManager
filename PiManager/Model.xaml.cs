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
using System.Windows.Threading;
using System.Diagnostics;

namespace PiManager
{
    public partial class Model : Window
    {
        static bool[] inputState = new bool[8];
        static bool[] outputState = new bool[8];

        static Thread modelThread = new Thread(new ThreadStart(StartProgram));
        static Model model = new Model();

        public Model()
        {
            InitializeComponent();
            model = this;
        }

        /**
         * Method is used to close all threads and the mainprocces to avoid exceptions
         * 
         * @param Sender
         * @param EventArgs
         */
        public static void CloseApp(Object sender, EventArgs e)
        {
            modelThread.Abort();
            model.Close();

            Process modelProcess = Process.GetCurrentProcess();
            modelProcess.Kill();
        }

        /**
         * This is the 'main method'. Mostly like the K8055-syntax.
         */
        public static void StartProgram()
        {
            for (int j = 1; j <= 8; j++)
            {
                SetDigitalChannel(j);
                Thread.Sleep(500);
                ClearDigitalChannel(j);
            }

            for (int j = 8; j >= 1; j--)
            {
                SetDigitalChannel(j);
                Thread.Sleep(500);
                ClearDigitalChannel(j);
            }
        }

        /**
         * Reads a digital input. Known from the K8055.
         * 
         * @param channel number
         * @return Returns channel state
         */
        public static bool ReadDigitalChannel(int channel)
        {
            return inputState[channel - 1];
        }

        /**
         * Clears all outputs. Known from the K8055.
         */
        public static void ClearAllDigital()
        {
            for (int i = 1; i <= 8; i++)
            {
                ClearDigitalChannel(i);
            }
        }

        /**
         * Clears a special output. Known from the K8055.
         * 
         * @param channel number
         */
        public static void ClearDigitalChannel(int channel)
        {
            Pin channelPin = new Pin();
            channelPin.IO = Pin.GPIO.o;
            channelPin.No = channel;
            channelPin.State = false;

            ChangeOutput(channelPin);
        }

        /**
         * Sets the state of a special output
         * 
         * @param channel number
         */
        public static void SetDigitalChannel(int channel)
        {
            Pin channelPin = new Pin();
            channelPin.IO = Pin.GPIO.o;
            channelPin.No = channel;
            channelPin.State = true;

            ChangeOutput(channelPin);
        }

        /**
         * When an input button in the user interface is pressed, this method runs.
         * 
         * @param sender
         * @param EventArgs
         */
        private void Input_Click(object sender, RoutedEventArgs e)
        {
            CheckBox input = (CheckBox)e.Source;
            string name = input.Content.ToString();

            ChangeInput(new Pin
            {
                IO = Pin.GPIO.i,
                No = Convert.ToInt32(name),
                State = Convert.ToBoolean(input.IsChecked)
            });
        }

        /**
         * Changes the input state, when a message from the stream has been read.
         * 
         * @param Pin to update
         */
        public static void ChangeInput(Pin pin)
        {
            inputState[pin.No - 1] = pin.State;

            Model model = new Model();
            switch (pin.No)
            {
                case 1:
                    model.input1.Dispatcher.Invoke((Action)(() => model.input1.IsChecked = pin.State));
                    break;
                case 2:
                    model.input2.Dispatcher.Invoke((Action)(() => model.input2.IsChecked = pin.State));
                    break;
                case 3:
                    model.input3.Dispatcher.Invoke((Action)(() => model.input3.IsChecked = pin.State));
                    break;
                case 4:
                    model.input4.Dispatcher.Invoke((Action)(() => model.input4.IsChecked = pin.State));
                    break;
                case 5:
                    model.input5.Dispatcher.Invoke((Action)(() => model.input5.IsChecked = pin.State));
                    break;
                case 6:
                    model.input6.Dispatcher.Invoke((Action)(() => model.input6.IsChecked = pin.State));
                    break;
                case 7:
                    model.input7.Dispatcher.Invoke((Action)(() => model.input7.IsChecked = pin.State));
                    break;
                case 8:
                    model.input8.Dispatcher.Invoke((Action)(() => model.input8.IsChecked = pin.State));
                    break;
            }
            model.input1.Refresh();
        }

        /**
         * Changes the output state in the user interface and broadcast it to the network streams
         * 
         * @param Pin to update
         */
        public static void ChangeOutput(Pin pin)
        {
            outputState[pin.No - 1] = pin.State;
            
            switch (pin.No)
            {
                case 1:
                    model.outputRadio1.Dispatcher.Invoke((Action)(() => model.outputRadio1.IsChecked = pin.State));
                    break;
                case 2:
                    model.outputRadio2.Dispatcher.Invoke((Action)(() => model.outputRadio2.IsChecked = pin.State));
                    break;
                case 3:
                    model.outputRadio3.Dispatcher.Invoke((Action)(() => model.outputRadio3.IsChecked = pin.State));
                    break;
                case 4:
                    model.outputRadio4.Dispatcher.Invoke((Action)(() => model.outputRadio4.IsChecked = pin.State));
                    break;
                case 5:
                    model.outputRadio5.Dispatcher.Invoke((Action)(() => model.outputRadio5.IsChecked = pin.State));
                    break;
                case 6:
                    model.outputRadio6.Dispatcher.Invoke((Action)(() => model.outputRadio6.IsChecked = pin.State));
                    break;
                case 7:
                    model.outputRadio7.Dispatcher.Invoke((Action)(() => model.outputRadio7.IsChecked = pin.State));
                    break;
                case 8:
                    model.outputRadio8.Dispatcher.Invoke((Action)(() => model.outputRadio8.IsChecked = pin.State));
                    break;
            }
            model.outputRadio1.Refresh();
            ConnectionHandler.BroadcastPin(pin);
        }

        /**
         * Start the model method. Threaded. 
         * 
         * @param sender
         * @param EventArgs
         */
        private void button_Click(object sender, RoutedEventArgs e)
        {
            modelThread.SetApartmentState(ApartmentState.STA);
            modelThread.IsBackground = true;
            modelThread.Start();
            button.IsEnabled = false;
        }

        /**
         * Handles the close button
         * 
         * @param sender
         * @param EventArgs
         */
        private void closeBTN_Click(object sender, RoutedEventArgs e)
        {
            CloseApp(null, EventArgs.Empty);
        }

        /**
         * Handles drag-drop. Gray bar on the top of the window.
         * 
         * @param sender
         * @param EventArgs
         */
        private void Rectangle_DragOver(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
