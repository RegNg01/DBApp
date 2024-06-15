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
    /// Логика взаимодействия для EditDry.xaml
    /// </summary>
    public partial class EditDry : Window
    {
        SQLAuth auth;
        public double startWetness;
        public double endWetness;
        public double startVolume;
        public double endVolume;
        public EditDry()
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
         
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            saveButton.IsEnabled = false;
            try
            {
                auth.connection.Open();
                if (
                        endWetnessTextBox.Text.Trim() == "" ||
                        endVolumeTextBox.Text.Trim() == ""
                        )
                {
                    MessageBox.Show("Заполните поля выхода");
                    saveButton.IsEnabled = true;
                    return;
                }
                if (!isValid(endVolumeTextBox.Text.Trim()) || !isValid(endWetnessTextBox.Text.Trim()))
                {

                    MessageBox.Show("Заполните поля числовыми значениями");
                    saveButton.IsEnabled = true;
                    return;
                }
                LumberModel lumberModel = this.DataContext as LumberModel;
                string sql = "INSERT INTO LumberStorage " +
                "(type, grade, wetness, volume, length, quantity, thickness, width, cut) " +
                 "VALUES ((SELECT id FROM TypeOfTree WHERE nameOfTheTreeType = @type), " +
                 "(SELECT id FROM TreeVariety WHERE name = @grade), @wetness, @volume, @length, @quantity, @thickness, @width, @cut)";

                MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
                cmd.Parameters.AddWithValue("@grade", lumberModel.Grade);
                cmd.Parameters.AddWithValue("@wetness", endWetness);
                cmd.Parameters.AddWithValue("@length", lumberModel.Length);
                cmd.Parameters.AddWithValue("@volume", endVolume);
                cmd.Parameters.AddWithValue("@quantity", lumberModel.Quantity);
                cmd.Parameters.AddWithValue("@type", lumberModel.Type);
                cmd.Parameters.AddWithValue("@thickness", lumberModel.Thickness);
                cmd.Parameters.AddWithValue("@width", lumberModel.Width);
                cmd.Parameters.AddWithValue("@cut", lumberModel.Cut);
                cmd.ExecuteNonQuery();
                string sql1 = "DELETE FROM DryStorage WHERE id = @id";

                MySqlCommand cmd1 = new MySqlCommand(sql1, auth.connection); 
                cmd1.Parameters.AddWithValue("@id", lumberModel.ID);
                cmd1.ExecuteNonQuery();
                MessageBox.Show("Отправлено в пиломатериалы");
                auth.connection.Close();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                if (auth.connection.State == System.Data.ConnectionState.Open)
                {
                    auth.connection.Close();
                }
            }
            saveButton.IsEnabled = true;
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
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
