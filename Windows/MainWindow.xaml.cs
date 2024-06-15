using DBApp.Pages;
using MySql.Data.MySqlClient;
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

namespace DBApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SQLAuth auth;
        public MainWindow()
        {
            InitializeComponent();
            auth = new SQLAuth();
            
        }

        private void enterButton_Click(object sender, RoutedEventArgs e)
        {
            if (loginTextBox.Text.Trim() != "" && passwordBox.Password.Trim() != "")
                try
            {
                enterButton.IsEnabled = false;
                auth.connection.Open();
                string sql = "SELECT COUNT(*) FROM `LoginDetails` " +
                    "WHERE login = @login AND password = @password";
                MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
                cmd.Parameters.AddWithValue("@login", loginTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@password", passwordBox.Password.Trim());
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count > 0)
                {
                    UserWindow userWindow = new UserWindow();
                MessageBox.Show($@"Вход под логином {loginTextBox.Text.Trim()}");
                userWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неправильный логин или пароль");
                }
                auth.connection.Close();
                enterButton.IsEnabled = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            else
                MessageBox.Show("Заполните все поля");
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

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            if (loginTextBox.Text.Trim() != "" && passwordBox.Password.Trim() != "")
                try
                {
                    registerButton.IsEnabled = false;
                    auth.connection.Open();
                    string line = "SELECT COUNT(*) FROM `LoginDetails` " +
                        "WHERE login = @login";
                    MySqlCommand check = new MySqlCommand(line, auth.connection);
                    check.Parameters.AddWithValue("@login", loginTextBox.Text.Trim());
                    int userExists = Convert.ToInt32(check.ExecuteScalar());
                    if (userExists > 0)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.");
                    }
                    else
                    {
                        string sql = "INSERT INTO `LoginDetails` (login, password, roleId) " +
                            "VALUES (@login, @password, @role)";
                        MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
                        cmd.Parameters.AddWithValue("@login", loginTextBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", passwordBox.Password.Trim());
                        cmd.Parameters.AddWithValue("@role", 2);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Регистрация успешна.");
                    }
                    auth.connection.Close();
                    registerButton.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    registerButton.IsEnabled = true;
                    auth.connection.Close();
                }
            else
                MessageBox.Show("Заполните все поля");
        }
         
    }
}
