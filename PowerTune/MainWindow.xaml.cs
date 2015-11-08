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

            ClsGetAdvData getAdvData = new ClsGetAdvData(); //create a new instance of ClsGetAdvData Class

            getAdvData.dataRequested += GetAdvData_dataRequested; //add event trigger 


            //define thread with lamdba expression, "normal" definition does not support more than one parameter.
            Thread threadGetAdvData = new Thread(() => getAdvData.request(libs.clsComSettings.strSelectCom, libs.clsComSettings.strSelectedBaud));
            threadGetAdvData.IsBackground = true;

            if (libs.clsComSettings.strSelectCom == "" || libs.clsComSettings.strSelectedBaud == 0)
            {
                //when serial settings are not set, bring configuration dialog up.
                Window wdwSetupCom = new Setup_Window.SetupCom();
                wdwSetupCom.Show();
            }
            else
            {
                threadGetAdvData.Start();
            }
        }

        private void GetAdvData_dataRequested()
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}




class ClsGetAdvData
{
    public delegate void serialDataRequested();
    public delegate void delRequestData(string comPort, int baudRate);
    public event serialDataRequested dataRequested; //event that will be fired when data is requested to inform parser

    private bool serialPfcReadyToRead = true; //this bool is set when data is ready to be requested and old data is processed out of serial buffer


    //open Serial Port with settings from clsComSettings class
    byte[] adv_request = { 0xF0, 0x02, 0x0D };   //Command for requesting advanced sensor data from PFC
    public void request(string comPort, int baudRate)
    {

        SerialPort serialPort = new SerialPort(comPort, baudRate);

        if (serialPort.IsOpen != true)
        {
            serialPort.Open();
        }
        if (serialPfcReadyToRead)
        {
            serialPort.Write(adv_request, 0, 3); // Write byte array to serial port, with no offset, all 3 bytes

            serialPfcReadyToRead = false;    //set to false till data is processed out of buffer
                                             //Here a event should be fired for a th
            dataRequested(); //Fire event!
        }
        serialPort.Close();
    }

}