using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO.Ports;
using System.IO;
using System.Net.Sockets;
using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace RS485Temperature
{
    public partial class MainWindow : Window
    {
        #region Fields
        private TcpClient _tcpClient;

        private NetworkStream _networkStream;
        #endregion

        private void Connect_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_tcpClient?.Connected != true)
                {
                    _tcpClient = new TcpClient(IPText.Text, 8016);
                }
            }
            catch (Exception ex)
            {
                (sender as ToggleButton).IsChecked = false;
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Connect_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_tcpClient?.Connected == true)
            {
                _networkStream.Close();
                _tcpClient.Close();
                _tcpClient = null;
            }
        }
    }
}
