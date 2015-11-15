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


            if (e.ProgressPercentage == 0)
            {
                sbStatus.Text = "Logging active...";
            }
            else
            {
                sbStatus.Text = "Offline";
            }

        }

        private void MenuItem_Click_Debugging(object sender, RoutedEventArgs e)
        {
            Window wdwDebugging = new Setup_Window.Debugging_Window();
            wdwDebugging.Show();
        }
    }


}




class ClsGetAdvData
{
    //open Serial Port with settings from clsComSettings class
    byte[] adv_request = { 0xF0, 0x02, 0x0D };   //Command for requesting advanced sensor data from PFC
    byte[] pfc_response = null; //Array for Data received from pfc
    int pfc_SizeOfPackage = 0;
    int pfc_checkSum = 0;



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
            serialPort.DataReceived += new SerialDataReceivedEventHandler(process_DoWorkHandler);
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
                    serialPort.DiscardInBuffer(); // Eingangspuffer leeren
                    serialPort.Write(adv_request, 0, 3); // Write byte array to serial port, with no offset, all 3 bytes

                }
            }
        }

    }

    public void process_DoWorkHandler(object sender, SerialDataReceivedEventArgs e)
    {
        // this function should parse the serial In Data
        // it should parse multiple versions of arrays
        // checksum test if package is transfered successfull
        // Function is still not robust enough

        Thread.Sleep(100); //sleep for 100ms to be sure all data from PFC is received
        SerialPort serialPort = (SerialPort)sender;

        pfc_response[0] = (byte)serialPort.ReadByte(); //First byte returns first byte of request, e.g. 0xF0 for advanced data
        pfc_response[1] = (byte)serialPort.ReadByte(); //Second byte indicates remaining size from data package, w/o first byte!!
        pfc_SizeOfPackage = pfc_response[1];
        for (int i = 0; pfc_SizeOfPackage <= i; i++) //loop trough array to get all data from serialInBuffer
        {
            pfc_response[i + 2] = (byte)serialPort.ReadByte();
        }
        //calculate Checksum from pfc_package: 0xFF minus each byte = last byte in array, if not -> discard
        pfc_checkSum = 0xFF;

        for (int i = 0; pfc_SizeOfPackage + 1 <= i; i++)
        {
            pfc_checkSum = pfc_checkSum - pfc_response[i];
        }
        if (pfc_checkSum == pfc_response[pfc_SizeOfPackage + 1])
        {

        }

    }


}



class ClsComSettingMain //this class is parameters for public void request_DoWork(object sender, DoWorkEventArgs e)
{
    public string comport { get; set; }
    public int baudRate { get; set; }
}