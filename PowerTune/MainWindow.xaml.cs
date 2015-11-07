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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.IO.Ports;

namespace PowerTune
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
   
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            sbStatus.Text = "Footer";



            //Load Configuration from settings stored in user profile
            libs.clsComSettings.strSelectCom = Properties.Settings.Default.ComPort;
            libs.clsComSettings.strSelectedBaud = int.Parse(Properties.Settings.Default.BaudRate);

        }

        private void getAdvData()
        {
            bool serialPfcReadToRead = true; //this bool is set when data is ready to be requested and old data is processed out of serial buffer

            //open Serial Port with settings from clsComSettings class
            byte[] adv_request = { 0xF0, 0x02, 0x0D };   //Command for requesting advanced sensor data from PFC

            //open Serialport with settings from class clsComSettings.cs
            SerialPort serialPort = new SerialPort(libs.clsComSettings.strSelectCom,
            libs.clsComSettings.strSelectedBaud);

            if (serialPort.IsOpen == false)
            {
                serialPort.Open(); 
            }
            if (serialPfcReadToRead)
            {
                serialPort.Write(adv_request, 0, 3); // Write byte array to serial port, with no offest, all 3 bytes
                serialPort.Close();
                serialPfcReadToRead = false;    //set to false till data is processed out of buffer
                //Here a event should be fired for a th
            }
     
        }   


        private void Update() //ONLY A THREAD TEST!!!!
        {

            int counter = 0;
            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                counter++;
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (ThreadStart)delegate ()
                        {

                            textBox.Text = counter.ToString();
                        }
                          );
            }

        }


        private void MenuItem_Click_Communication(object sender, RoutedEventArgs e)
        {

            Window wdwSetupCom = new Setup_Window.SetupCom();
            wdwSetupCom.Show();

        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {

            Close();

        }

        private void MenuItem_Click_Serial_Start(object sender, RoutedEventArgs e)
        {
            Thread serialWorker = new Thread(getAdvData); //instanciate a new thread
            serialWorker.IsBackground = true;

            if (libs.clsComSettings.strSelectCom == "" || libs.clsComSettings.strSelectedBaud == 0)
            {
                //when serial settings are not set, bring configuration dialog up.
                Window wdwSetupCom = new Setup_Window.SetupCom();
                wdwSetupCom.Show();
            }
            else
            {
                serialWorker.Start(); //start Worker Thread
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
