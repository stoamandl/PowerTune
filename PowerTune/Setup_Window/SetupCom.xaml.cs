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
using System.IO.Ports;

namespace PowerTune.Setup_Window
{
    /// <summary>
    /// Interaktionslogik für SetupCom.xaml
    /// </summary>
    public partial class SetupCom : Window
    {
        public SetupCom()
        {
            InitializeComponent();

            string[] ArrayComPortsNames = null;

            ArrayComPortsNames = SerialPort.GetPortNames();

            foreach (string s in ArrayComPortsNames)
            {
                cboComPorts.Items.Add(s);
            }

            cboBaudRates.Items.Add(300);
            cboBaudRates.Items.Add(600);
            cboBaudRates.Items.Add(1200);
            cboBaudRates.Items.Add(2400);
            cboBaudRates.Items.Add(9600);
            cboBaudRates.Items.Add(14400);
            cboBaudRates.Items.Add(19200);
            cboBaudRates.Items.Add(38400);
            cboBaudRates.Items.Add(57600);
            cboBaudRates.Items.Add(115200);
            cboBaudRates.Items.ToString();

        }

        private void btnRescan_Click(object sender, RoutedEventArgs e)
        {
            cboComPorts.Items.Clear();
            string[] ArrayComPortsNames = null;

            ArrayComPortsNames = SerialPort.GetPortNames();
            foreach (string s in ArrayComPortsNames)
            {
                cboComPorts.Items.Add(s);
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            libs.clsComSettings.strSelectCom = cboComPorts.Text;
            Properties.Settings.Default["ComPort"] = cboComPorts.Text;
            libs.clsComSettings.strSelectedBaud = cboBaudRates.Text;
            Properties.Settings.Default["BaudRate"] = cboBaudRates.Text;
            Properties.Settings.Default.Save();

            this.Close();
        }
    }
}
