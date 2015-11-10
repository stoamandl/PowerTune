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

            ClsGetAdvData getAdvData = new ClsGetAdvData(); //create a new instance of ClsGetAdvData Class



            //initialize the workerGetAdvData Background Worker
            workerGetAdvData = new BackgroundWorker();
            workerGetAdvData.WorkerSupportsCancellation = true;
            workerGetAdvData.WorkerReportsProgress = true;


            workerGetAdvData.DoWork += new DoWorkEventHandler(getAdvData.request_DoWork);
            workerGetAdvData.ProgressChanged += new ProgressChangedEventHandler(request_ProgressChanged); 

            InitializeComponent();
            sbStatus.Text = "Offline";
            


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

            ClsComSettingMain clsComSettingMain = new ClsComSettingMain();

            clsComSettingMain.comport = libs.clsComSettings.strSelectCom;
            clsComSettingMain.baudRate = libs.clsComSettings.strSelectedBaud;




            if (libs.clsComSettings.strSelectCom == "" || libs.clsComSettings.strSelectedBaud == 0)
            {
                //when serial settings are not set, bring configuration dialog up.
                Window wdwSetupCom = new Setup_Window.SetupCom();
                wdwSetupCom.Show();
            }
            else
            {
                if (!workerGetAdvData.IsBusy)
                {
                    workerGetAdvData.RunWorkerAsync(clsComSettingMain);
                }
            }
        }

        private void MenuItem_Click_Serial_Stop(object sender, RoutedEventArgs e)
        {
            if (workerGetAdvData.IsBusy) //check if worker is running
            {
                workerGetAdvData.CancelAsync();
            }
        }


        public void request_ProgressChanged(object sender, ProgressChangedEventArgs e) //this function is need to set statusbartext
        {
            

            if(e.ProgressPercentage == 0)
            {
                sbStatus.Text = "Logging active...";
            }
            else
            {
                sbStatus.Text = "Offline";
            }


            // parser could be triggered here..?
        }
    }


}




class ClsGetAdvData
{
    //open Serial Port with settings from clsComSettings class
    byte[] adv_request = { 0xF0, 0x02, 0x0D };   //Command for requesting advanced sensor data from PFC



    public void request_DoWork(object sender, DoWorkEventArgs e)
    {
        var worker = (BackgroundWorker)sender;

        worker.ReportProgress(0);


        ClsComSettingMain clsComSettingMain = (ClsComSettingMain)e.Argument;
        string comPort = clsComSettingMain.comport;
        int baudRate = clsComSettingMain.baudRate;

        if (comPort != null && baudRate != 0)
        {
            SerialPort serialPort = new SerialPort(comPort, baudRate);
            serialPort.Open();

            while (true)
            {
                if (worker.CancellationPending)
                {
                    worker.ReportProgress(100);
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