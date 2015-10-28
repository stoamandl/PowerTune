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
            Properties.Settings.Default["ComPort"] =  cboComPorts.Text;
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
