using DBApp.Pages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для EditLog.xaml
    /// </summary>
    public partial class EditLog : Window
    {
        SQLAuth auth;
         

        public EditLog()
        {
            InitializeComponent();
            auth = new SQLAuth(); 
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
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

        void saveButton_Click(object sender, RoutedEventArgs e)
        {
            saveButton.IsEnabled = false;
            try
            {
                auth.connection.Open();
                if (diameter1TextBox.Text.Trim() == "" || 
                    lengthTextBox.Text.Trim() == "" ||
                    volumeTextBox.Text.Trim() == "" ||
                    wetnessTextBox.Text.Trim() == "" ||
                    gradeCMB.SelectedIndex == -1||
                    typeCMB.SelectedIndex == -1 ||
                    quantityTextBox.Text.Trim()==""||
                    lengthTextBox.Text.Trim()==""
                    )
                {
                    MessageBox.Show("Заполните все поля");
                    saveButton.IsEnabled = true;
                    return;
                }
                if (!isValid(lengthTextBox.Text.Trim())||!isValid(diameter1TextBox.Text.Trim()) || (diameter2TextBox.Text.Trim() != ""&&diameter2TextBox.Text.Trim() != "0" && !isValid(diameter2TextBox.Text.Trim())) || !isValid(wetnessTextBox.Text.Trim()) || !isValid(volumeTextBox.Text.Trim()) || !isValid(quantityTextBox.Text.Trim()))
                {
                    MessageBox.Show("Заполните поля числовыми значениями");
                    saveButton.IsEnabled = true;
                    return;
                } 
                LogModel logModel = this.DataContext as LogModel;
                    string diameterRange = string.IsNullOrWhiteSpace(diameter2TextBox.Text)
                ? diameter1TextBox.Text.Trim()
                : $"{diameter1TextBox.Text.Trim()}...{diameter2TextBox.Text.Trim()}";

                string sql = "UPDATE WoodStorage AS ws " + 
                             "SET ws.diameter1 = @diameter1, " +
                             "ws.diameter2 = @diameter2, " +
                             "ws.type = (SELECT id FROM TypeOfTree WHERE nameOfTheTreeType = @type), " +
                             "ws.grade = (SELECT id FROM TreeVariety WHERE name = @grade), " +
                             "ws.wetness = @wetness, " +
                             "ws.volume = @volume, " +
                             "ws.length = @length, " + 
                             "ws.quantity = @quantity " +
                             "WHERE ws.id = @id";
                
                MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
                cmd.Parameters.AddWithValue("@diameter1", logModel.Diameter1);
                cmd.Parameters.AddWithValue("@diameter2", logModel.Diameter2);
                cmd.Parameters.AddWithValue("@grade", logModel.Grade);
                cmd.Parameters.AddWithValue("@wetness", logModel.Wetness); 
                cmd.Parameters.AddWithValue("@length", logModel.Length);
                cmd.Parameters.AddWithValue("@quantity", logModel.Quantity);
                cmd.Parameters.AddWithValue("@volume", logModel.Volume);
                cmd.Parameters.AddWithValue("@type", logModel.Type);
                cmd.Parameters.AddWithValue("@id", logModel.ID);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Данные успешно сохранены");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных: " + ex.Message);
            }
            finally
            {
                if (auth.connection.State == System.Data.ConnectionState.Open)
                {
                    auth.connection.Close();
                }
            }
            this.Close(); 
            saveButton.IsEnabled = true;
        }
         
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private bool isValid(string str)
        {
            bool point = false;
            str = str.ToLower();

            if (str.StartsWith("."))
            {
                return false;
            } 
            foreach (char ch in str)
            {
                if (!(char.IsDigit(ch) || ch == '.'))
                {
                    return false;
                }
                if (ch == '.')
                {
                    if (point)
                    {
                        return false;
                    }
                    point = true;
                }
            } 
            if (double.TryParse(str, out double number))
            {
                if (number == 0)
                {
                    return false;
                }
            }
            else
            {
                return false;
            } 
            return true;
        } 
         
        private void SetVolume()
        {
            if (int.TryParse(quantityTextBox.Text, out int quantity) &&
             double.TryParse(diameter1TextBox.Text, out double diameter1) &&
             double.TryParse(lengthTextBox.Text, out double length))
            {
                double volume = 0;

                if (string.IsNullOrWhiteSpace(diameter2TextBox.Text))
                {
                    volume = quantity * Calculate(diameter1, length);
                }
                else if (double.TryParse(diameter2TextBox.Text, out double diameter2))
                {
                    volume = quantity * Calculate((diameter1 + diameter2) / 2, length);
                }

                volumeTextBox.Text = Math.Round(volume, 2).ToString();
                volumeTextBox.InvalidateVisual();
            }
        }

        private double Calculate(double diameter, double length)
        {
            double volume = 3.14 * ((diameter / 1000) / 2) * ((diameter / 1000) / 2) * length;
            return volume;
        }

        private void quantityTextBox_LostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SetVolume();
        }
         
        private void addButton_Click(object sender, RoutedEventArgs e)
        { 
                addButton.IsEnabled = false;
                try
                {
                auth.connection.Open();
                if (diameter1TextBox.Text.Trim() == "" || 
                        lengthTextBox.Text.Trim() == "" ||
                        volumeTextBox.Text.Trim() == "" ||
                        wetnessTextBox.Text.Trim() == "" ||
                        gradeCMB.SelectedIndex == -1 ||
                        typeCMB.SelectedIndex == -1 ||
                        quantityTextBox.Text.Trim() == "" ||
                        lengthTextBox.Text.Trim() == ""
                        )
                    {
                        MessageBox.Show("Заполните все поля");
                        addButton.IsEnabled = true;
                        return;
                    }
                    if (!isValid(lengthTextBox.Text.Trim()) || !isValid(diameter1TextBox.Text.Trim()) || (diameter2TextBox.Text.Trim() != "" && diameter2TextBox.Text.Trim() != "0" && !isValid(diameter2TextBox.Text.Trim())) || !isValid(wetnessTextBox.Text.Trim()) || !isValid(volumeTextBox.Text.Trim()) || !isValid(quantityTextBox.Text.Trim()))
                    { 
                     
                        MessageBox.Show("Заполните поля числовыми значениями");
                        addButton.IsEnabled = true;
                        return;
                     }
                    LogModel logModel = this.DataContext as LogModel; 
                    string sql = "INSERT INTO WoodStorage " +
                    "(diameter1, diameter2, type, grade, wetness, volume, length, quantity) " +
                     "VALUES (@diameter1, @diameter2, " +
                     "(SELECT id FROM TypeOfTree WHERE nameOfTheTreeType = @type), " +
                     "(SELECT id FROM TreeVariety WHERE name = @grade), @wetness, @volume, @length, @quantity)";

                MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
                    cmd.Parameters.AddWithValue("@diameter1", logModel.Diameter1);
                    cmd.Parameters.AddWithValue("@diameter2", logModel.Diameter2);
                    cmd.Parameters.AddWithValue("@grade", logModel.Grade);
                    cmd.Parameters.AddWithValue("@wetness", logModel.Wetness); 
                    cmd.Parameters.AddWithValue("@length", logModel.Length);
                    cmd.Parameters.AddWithValue("@volume", logModel.Volume);
                    cmd.Parameters.AddWithValue("@quantity", logModel.Quantity);
                    cmd.Parameters.AddWithValue("@type", logModel.Type); 
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Данные успешно сохранены"); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении данных: " + ex.Message);
                }
                finally
                {
                    if (auth.connection.State == System.Data.ConnectionState.Open)
                    {
                        auth.connection.Close();
                    }
                }
            this.Close();
                addButton.IsEnabled = true;
            
        }
        
    }
}
