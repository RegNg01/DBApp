using DBApp.Pages;
using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace DBApp
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public UserWindow()
        {
            InitializeComponent();
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void navBarListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (navBarListView.SelectedItem != null)
            {
                ListViewItem selectedItem = (ListViewItem)navBarListView.SelectedItem;
                int tag = int.Parse(selectedItem.Tag.ToString());

                switch (tag)
                {
                    case 1:
                        navFrame.Content = new Log();
                        break;
                    case 2:
                        navFrame.Content = new Dry();
                        break;
                    case 3:
                        navFrame.Content = new Saw();
                        break;
                    case 4:
                        navFrame.Content = new Storage();
                        break;
                    default:
                        break;
                }
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
