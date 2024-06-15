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

namespace DBApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditToDry.xaml
    /// </summary>
    public partial class EditToDry : Window
    {
        SQLAuth auth;
        public int startQuantity;
        public double startVolume;
        public double endVolume;
        public int endQuantity;
        public double sizeVolume;
        public EditToDry()
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
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            saveButton.IsEnabled = false;
            try
            {
                auth.connection.Open();
                if ( 
                        quantityTextBox.Text.Trim() == "" ||
                        volumeTextBox.Text.Trim() == ""
                        )
                {
                    MessageBox.Show("Заполните все поля");
                    saveButton.IsEnabled = true;
                    return;
                }
                if (!isValid(volumeTextBox.Text.Trim()) || !isValid(quantityTextBox.Text.Trim()))
                {

                    MessageBox.Show("Заполните поля числовыми значениями");
                   saveButton.IsEnabled = true;
                    return;
                }
                LumberModel lumberModel = this.DataContext as LumberModel;
                string sql = "INSERT INTO DryStorage " +
                "(type, grade, wetness, volume, length, quantity, thickness, width, cut) " +
                 "VALUES ((SELECT id FROM TypeOfTree WHERE nameOfTheTreeType = @type), " +
                 "(SELECT id FROM TreeVariety WHERE name = @grade), @wetness, @volume, @length, @quantity, @thickness, @width, @cut)";

                MySqlCommand cmd = new MySqlCommand(sql, auth.connection); 
                cmd.Parameters.AddWithValue("@grade", lumberModel.Grade);
                cmd.Parameters.AddWithValue("@wetness", lumberModel.Wetness);
                cmd.Parameters.AddWithValue("@length", lumberModel.Length);
                cmd.Parameters.AddWithValue("@volume", lumberModel.Volume);
                cmd.Parameters.AddWithValue("@quantity", lumberModel.Quantity);
                cmd.Parameters.AddWithValue("@type", lumberModel.Type);
                cmd.Parameters.AddWithValue("@thickness", lumberModel.Thickness);
                cmd.Parameters.AddWithValue("@width", lumberModel.Width);
                cmd.Parameters.AddWithValue("@cut", lumberModel.Cut);
                cmd.ExecuteNonQuery();
                
                string sql1 = "UPDATE `LumberStorage` SET quantity = @quantity, " +
                  "volume = @volume WHERE LumberStorage.id = @id";
                MySqlCommand cmd1 = new MySqlCommand(sql1, auth.connection);
                cmd1.Parameters.AddWithValue("@quantity", startQuantity - endQuantity);
                cmd1.Parameters.AddWithValue("@id", lumberModel.ID);
                cmd1.Parameters.AddWithValue("@volume", Math.Round(startVolume - endVolume, 2));
                cmd1.ExecuteNonQuery();
                if (startQuantity - endQuantity == 0 || startVolume - endVolume == 0)
                {
                    string sql3 = "DELETE FROM LumberStorage WHERE LumberStorage.id = @id";
                    MySqlCommand cmd3 = new MySqlCommand(sql3, auth.connection);
                    cmd3.Parameters.AddWithValue("@id", lumberModel.ID);
                    cmd3.ExecuteNonQuery();
                }
                MessageBox.Show("Отправлено на сушку");
                auth.connection.Close();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            saveButton.IsEnabled = true;
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            TextBox textBox = sender as TextBox;
            if (int.TryParse(textBox.Text + e.Text, out int value))
            {

                if (value > startQuantity)
                {

                    e.Handled = true;
                }
            }
            if (int.TryParse(quantityTextBox.Text, out int quantityValue) && quantityTextBox.Text.Trim() != "")
            {
                endQuantity = quantityValue;
                if (endQuantity > 0)
                {
                    endVolume = sizeVolume * endQuantity;
                    volumeTextBox.Text = Math.Round(endVolume, 2).ToString();
                }
                else
                {
                    endVolume = 0;
                }
            }
            else
                volumeTextBox.Text = "";

        }
        private void TextBox_PreviewTextInput2(object sender, TextCompositionEventArgs e)
        {

            TextBox textBox = sender as TextBox;
            if (double.TryParse(textBox.Text + e.Text, out double value))
            {

                if (value > startVolume)
                {

                    e.Handled = true;
                }
            }
            if (double.TryParse(volumeTextBox.Text, out double volumeValue) && volumeTextBox.Text.Trim() != "")
            {
                endVolume = volumeValue;
                if (sizeVolume != 0)
                {
                    endQuantity = Convert.ToInt32(Math.Floor(endVolume / sizeVolume));
                    quantityTextBox.Text = endQuantity.ToString();
                }
            }
            else quantityTextBox.Text = "";

        }
    }
}
