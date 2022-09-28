using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;


namespace RS485Temperature
{
    public partial class MainWindow : Window
    {
        private void TitleGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.ClickCount >= 2)
            //{
            //    Maxbtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            //}
            //else
            //{
            //    DragMove();
            //}
            DragMove();
        }

        private void Minbtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maxbtn_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void Quitbtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
