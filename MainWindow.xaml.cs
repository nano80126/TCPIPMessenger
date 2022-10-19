using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RS485Temperature
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private Thermometer Thermometer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Thermometer = FindResource("Thermometer") as Thermometer;
            //RefreshBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            Dictionary<int, int> dict = new Dictionary<int, int>();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if (Thermometer.IsSerialPortOpen)
            //{
            //    Thermometer.CloseSerialPort();
            //}
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_tcpClient?.Connected == true)
                {

                    _networkStream = _tcpClient.GetStream();

                    byte[] data = Encoding.UTF8.GetBytes(MsgTextBox.Text);
                    _networkStream.Write(data, 0, data.Length);


                    byte[] res = new byte[256];
                    int i = _networkStream.Read(res, 0, res.Length);

                    System.Diagnostics.Debug.WriteLine($"{i} {_networkStream.DataAvailable}");
                    string msg = Encoding.UTF8.GetString(res, 0, i);
                    System.Diagnostics.Debug.WriteLine($"{msg}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                TcpConnect.IsChecked = false;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
            TitleGrid.Focus();
        }
    }
}
