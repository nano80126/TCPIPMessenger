﻿using System;
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
            if (_tcpClient?.Connected == true)
            {
                _networkStream = _tcpClient.GetStream();

                byte[] data = Encoding.UTF8.GetBytes(MsgTextBox.Text);
                _networkStream.Write(data, 0, data.Length);
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
            TitleGrid.Focus();
        }
    }
}