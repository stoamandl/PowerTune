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
            Thread worker = new Thread(Update);
            worker.Start();
        }

        private void Update()
    {
        // Simulate some work taking place 
        Thread.Sleep(TimeSpan.FromSeconds(5));
        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate ()
                    {

                     textBox.Text = "Here is some new text.";
                    }
                      );

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
