using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для EditLumber.xaml
    /// </summary>
    public partial class EditLumber : Window
    {
        SQLAuth auth;
        public EditLumber()
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
                if (thicknessTextBox.Text.Trim() == "" ||
                    lengthTextBox.Text.Trim() == "" ||
                    widthTextBox.Text.Trim() == "" ||
                    volumeTextBox.Text.Trim() == "" ||
                    wetnessTextBox.Text.Trim() == "" ||
                    gradeCMB.SelectedIndex == -1 ||
                    typeCMB.SelectedIndex == -1 ||
                    quantityTextBox.Text.Trim() == "" ||
                    lengthTextBox.Text.Trim() == ""
                    )
                {
                    MessageBox.Show("Заполните все поля");
                    saveButton.IsEnabled = true;
                    return;
                }
                if (!isValid(lengthTextBox.Text.Trim()) || !isValid(widthTextBox.Text.Trim()) || !isValid(thicknessTextBox.Text.Trim()) || !isValid(wetnessTextBox.Text.Trim()) || !isValid(volumeTextBox.Text.Trim()) || !isValid(quantityTextBox.Text.Trim()))
                {
                    MessageBox.Show("Заполните поля числовыми значениями");
                    saveButton.IsEnabled = true;
                    return;
                }
                LumberModel lumberModel = this.DataContext as LumberModel;
                string sql = "UPDATE LumberStorage AS lw " +
                             "SET lw.thickness = @thickness, " +
                             "lw.type = (SELECT id FROM TypeOfTree WHERE nameOfTheTreeType = @type), " +
                             "lw.grade = (SELECT id FROM TreeVariety WHERE name = @grade), " +
                             "lw.width = @width, " +
                             "lw.wetness = @wetness, " +
                             "lw.volume = @volume, " +
                             "lw.length = @length, " +
                             "lw.quantity = @quantity " +
                             "WHERE lw.id = @id";

                MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
                cmd.Parameters.AddWithValue("@thickness", lumberModel.Thickness);
                cmd.Parameters.AddWithValue("@width", lumberModel.Width);
                cmd.Parameters.AddWithValue("@grade", lumberModel.Grade);
                cmd.Parameters.AddWithValue("@wetness", lumberModel.Wetness);
                cmd.Parameters.AddWithValue("@length", lumberModel.Length);
                cmd.Parameters.AddWithValue("@quantity", lumberModel.Quantity);
                cmd.Parameters.AddWithValue("@volume", lumberModel.Volume);
                cmd.Parameters.AddWithValue("@type", lumberModel.Type);
                cmd.Parameters.AddWithValue("@id", lumberModel.ID);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Данные успешно сохранены");
                auth.connection.Close();
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

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            addButton.IsEnabled = false;
            try
            {
                auth.connection.Open();
                if (thicknessTextBox.Text.Trim() == "" ||
                    lengthTextBox.Text.Trim() == "" ||
                    widthTextBox.Text.Trim() == "" ||
                    volumeTextBox.Text.Trim() == "" ||
                    wetnessTextBox.Text.Trim() == "" ||
                    gradeCMB.SelectedIndex == -1 ||
                    typeCMB.SelectedIndex == -1 ||
                    quantityTextBox.Text.Trim() == "" ||
                    lengthTextBox.Text.Trim() == ""
                    )
                {
                    MessageBox.Show("Заполните все поля");
                    saveButton.IsEnabled = true;
                    return;
                }
                if (!isValid(lengthTextBox.Text.Trim()) || !isValid(widthTextBox.Text.Trim()) || !isValid(thicknessTextBox.Text.Trim()) || !isValid(wetnessTextBox.Text.Trim()) || !isValid(volumeTextBox.Text.Trim()) || !isValid(quantityTextBox.Text.Trim()))
                {
                    MessageBox.Show("Заполните поля числовыми значениями");
                    saveButton.IsEnabled = true;
                    return;
                }
                LumberModel lumberModel = this.DataContext as LumberModel;
                string sql = "INSERT INTO LumberStorage " +
                             "(type, grade, width, thickness, wetness, volume, length, quantity, cut) "+
                              "VALUES ((SELECT id FROM TypeOfTree WHERE nameOfTheTreeType = @type), "+
                             "(SELECT id FROM TreeVariety WHERE name = @grade), " +
                             "@width, @thickness, @wetness, @volume, @length, @quantity, @cut)";

                MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
                cmd.Parameters.AddWithValue("@thickness", lumberModel.Thickness);
                cmd.Parameters.AddWithValue("@width", lumberModel.Width);
                cmd.Parameters.AddWithValue("@grade", lumberModel.Grade);
                cmd.Parameters.AddWithValue("@wetness", lumberModel.Wetness);
                cmd.Parameters.AddWithValue("@length", lumberModel.Length);
                cmd.Parameters.AddWithValue("@quantity", lumberModel.Quantity);
                cmd.Parameters.AddWithValue("@volume", lumberModel.Volume);
                cmd.Parameters.AddWithValue("@type", lumberModel.Type);
                cmd.Parameters.AddWithValue("@cut", "1");
               
                cmd.ExecuteNonQuery();
                MessageBox.Show("Данные успешно сохранены");
                auth.connection.Close();
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
