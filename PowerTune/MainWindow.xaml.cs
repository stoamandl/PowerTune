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
            sbStatus.Text = "Offline";
            
            //Load Configuration from settings stored in user profile
            libs.clsComSettings.strSelectCom = Properties.Settings.Default.ComPort;
            libs.clsComSettings.strSelectedBaud = Properties.Settings.Default.BaudRate;

            Thread worker = new Thread(Update);
            worker.Start(); //start Worker Thread

        }

        private void getAdvData()
        {
            
        }


        private void Update()
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
    }
}
