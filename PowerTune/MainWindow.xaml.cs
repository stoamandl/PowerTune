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
using System.ComponentModel;

namespace PowerTune
{
    public partial class MainWindow : Window
    {
        BackgroundWorker workerGetAdvData;

        public MainWindow()
        {
            workerGetAdvData = new BackgroundWorker();

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

            ClsComSettingMain clsComSettingMain = new ClsComSettingMain();

            clsComSettingMain.comport = libs.clsComSettings.strSelectCom;
            clsComSettingMain.baudRate = libs.clsComSettings.strSelectedBaud;

            workerGetAdvData.DoWork += new DoWorkEventHandler(getAdvData.request_DoWork);
            //workerGetAdvData.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerGetAdvData_RunWorkerCompleted);

            workerGetAdvData.WorkerReportsProgress = true;
            workerGetAdvData.WorkerSupportsCancellation = true;


            if (libs.clsComSettings.strSelectCom == "" || libs.clsComSettings.strSelectedBaud == 0)
            {
                //when serial settings are not set, bring configuration dialog up.
                Window wdwSetupCom = new Setup_Window.SetupCom();
                wdwSetupCom.Show();
            }
            else
            {
                workerGetAdvData.RunWorkerAsync(clsComSettingMain);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (workerGetAdvData.IsBusy)
            {
                workerGetAdvData.CancelAsync();
            }
        }
    }
}




class ClsGetAdvData
{
    //open Serial Port with settings from clsComSettings class
    byte[] adv_request = { 0xF0, 0x02, 0x0D };   //Command for requesting advanced sensor data from PFC

    public void request_DoWork(object sender, DoWorkEventArgs e)
    {
        ClsComSettingMain clsComSettingMain = (ClsComSettingMain)e.Argument;
        string comPort = clsComSettingMain.comport;
        int baudRate = clsComSettingMain.baudRate;

        if (comPort != null && baudRate != 0)
        {
            SerialPort serialPort = new SerialPort(comPort, baudRate);
            serialPort.Open();

            while (true)
            {
                if (e.Cancel)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    serialPort.Write(adv_request, 0, 3); // Write byte array to serial port, with no offset, all 3 bytes
                    Thread.Sleep(500);
                }
            }
        }

    }


}



class ClsComSettingMain //this class is parameters for public void request_DoWork(object sender, DoWorkEventArgs e)
{
    public string comport { get; set; }
    public int baudRate { get; set; }
}