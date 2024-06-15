using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Org.BouncyCastle.Tls;
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
    /// Логика взаимодействия для EditSaw.xaml
    /// </summary>
    public partial class EditSaw : Window
    {
        SQLAuth auth;
        public double maxQuantity;
        public double sizeVolume;
        public double maxVolume;
        public double diameter;
        public double cutVolume;
        public int cutQuantity;

        public double resultVolume;
        public int resultQuantity;
        public EditSaw()
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

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                auth.connection.Open();
                LogModel logModel = this.DataContext as LogModel;
                if (widthCMB.Text == "" || thicknessCMB.Text == "" || !(Convert.ToDouble(resultVolumeTextBox.Text.Trim()) > 0) || !(Convert.ToInt32(resultQuantityTextBox.Text.Trim()) > 0))
                {
                    MessageBox.Show("Заполните все поля");
                    saveButton.IsEnabled = true;
                    return;
                }
                double width = Convert.ToDouble(widthCMB.Text);
                double thickness = Convert.ToDouble(thicknessCMB.Text);
                LumberModel lumberModel = new LumberModel
                {
                    Type = logModel.Type,
                    Grade = logModel.Grade,
                    Width = width,
                    Thickness = thickness,
                    Length = logModel.Length,
                    Wetness = logModel.Wetness,
                    Quantity = resultQuantity,
                    Volume = resultVolume,
                    Cut = false,
                    volumelog=Math.Round(cutVolume,2)
                };
                string sql = "INSERT INTO `SawStorage` " +
                    "( `volumelog`,`type`, `grade`, `width`, `thickness`, `length`, " +
                    "`wetness`, `quantity`, `volume`, `cut`) " +
                    "VALUES (@volumelog, (SELECT id FROM TypeOfTree WHERE nameOfTheTreeType = " +
                    "@type), (SELECT id FROM TreeVariety WHERE name = @grade), @width, " +
                    "@thickness, @length, @wetness, " +
                    "@quantity, @volume, @cut)";
                MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
                cmd.Parameters.AddWithValue("@volumelog", cutVolume);
                cmd.Parameters.AddWithValue("@type", lumberModel.Type);
                cmd.Parameters.AddWithValue("@grade", lumberModel.Grade);
                cmd.Parameters.AddWithValue("@width", lumberModel.Width);
                cmd.Parameters.AddWithValue("@thickness", lumberModel.Thickness);
                cmd.Parameters.AddWithValue("@length", lumberModel.Length);
                cmd.Parameters.AddWithValue("@wetness", lumberModel.Wetness);
                cmd.Parameters.AddWithValue("@quantity", lumberModel.Quantity);
                cmd.Parameters.AddWithValue("@volume", lumberModel.Volume);
                cmd.Parameters.AddWithValue("@cut", lumberModel.Cut);
                cmd.ExecuteNonQuery();

                string sql1 = "UPDATE `WoodStorage` SET quantity = @quantity, " +
                    "volume = @volume WHERE WoodStorage.id = @id";
                MySqlCommand cmd1 = new MySqlCommand(sql1, auth.connection);
                cmd1.Parameters.AddWithValue("@quantity", logModel.Quantity - cutQuantity);
                cmd1.Parameters.AddWithValue("@id", logModel.ID);
                cmd1.Parameters.AddWithValue("@volume", Math.Round(logModel.Volume - cutVolume, 2));
                cmd1.ExecuteNonQuery();
                if (logModel.Quantity - cutQuantity == 0 || logModel.Volume - cutVolume == 0)
                {
                    string sql3 = "DELETE FROM WoodStorage WHERE WoodStorage.id = @id";
                    MySqlCommand cmd3 = new MySqlCommand(sql3, auth.connection);
                    cmd3.Parameters.AddWithValue("@id", logModel.ID);
                    cmd3.ExecuteNonQuery();
                }
                MessageBox.Show("Отправлено на распиловку"); 
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (auth.connection.State == System.Data.ConnectionState.Open)
                {
                    auth.connection.Close();
                }
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
             
            TextBox textBox = sender as TextBox;
            if (int.TryParse(textBox.Text + e.Text, out int value))
            {
              
                if (value > maxQuantity)
                {
                 
                    e.Handled = true;
                }
            }
            if (int.TryParse(logQuantityTextBox.Text, out int quantityValue) && logQuantityTextBox.Text.Trim() != "")
            {
                cutQuantity = quantityValue;
                if (cutQuantity > 0)
                {
                    cutVolume = sizeVolume * cutQuantity;
                    cutVolumeTextBox.Text = Math.Round(cutVolume,2).ToString();
                }
                else
                {
                    cutVolume = 0;
                }
            }
            else
                cutVolumeTextBox.Text = "";

        }
        private void TextBox_PreviewTextInput2(object sender, TextCompositionEventArgs e)
        {
            
            TextBox textBox = sender as TextBox;
            if (double.TryParse(textBox.Text + e.Text, out double value))
            {

                if (value > maxVolume)
                {

                    e.Handled = true;
                }
            }
            if (double.TryParse(cutVolumeTextBox.Text, out double cutValue) && cutVolumeTextBox.Text.Trim() != "")
            {
                cutVolume = cutValue;
                if (sizeVolume != 0)
                {
                    cutQuantity = Convert.ToInt32(Math.Floor(cutVolume / sizeVolume));
                    logQuantityTextBox.Text = cutQuantity.ToString();
                }
            }
            else logQuantityTextBox.Text = "";

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LogModel logModel = this.DataContext as LogModel;
            cutVolume = 0;
            cutQuantity = 0;
            
            if (logModel != null)
            {
                if (logModel.Diameter2 != 0)
                    diameter = (logModel.Diameter1 + logModel.Diameter2) / 2;
                else diameter = logModel.Diameter1;
                maxQuantity = logModel.Quantity; //маскимально возможное количество бревен
                sizeVolume = logModel.Size; //объем одного бревна
                maxVolume = logModel.Volume; //маскимально возможный объем бревен

                for (int i = widthCMB.Items.Count - 1; i >= 0; i--)
                {
                    ComboBoxItem item = widthCMB.Items[i] as ComboBoxItem;
                    if (item != null && double.TryParse(item.Content.ToString(), out double value) && value > diameter)
                    {
                        widthCMB.Items.RemoveAt(i);
                    }
                }
            }

        }


        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            try {  
             if (widthCMB.Text == "" || thicknessCMB.Text == ""||cutVolumeTextBox.Text.Trim()==""||logQuantityTextBox.Text.Trim()=="")
                {
                MessageBox.Show("Заполните все поля");
                return;
                }
                double widthValue = Convert.ToDouble(widthCMB.Text);
                if (widthValue > diameter)
                {
                    MessageBox.Show($"Значение ширины не может превышать {diameter} мм");
                    return;
                }
                LogModel logModel = this.DataContext as LogModel;
            switch (logModel.Grade)
            {
                case "A":
                    resultVolume = Math.Round((cutVolume / 2),2);
                    break;
                case "B":
                    resultVolume = Math.Round((cutVolume / 4), 2);
                    break;
                case "C":
                    resultVolume = Math.Round((cutVolume / 4), 2);
                    break;
                default: resultVolume = Math.Round((cutVolume / 4), 2); break;
            }
            double width = Convert.ToDouble(widthCMB.Text) / 1000;  
            double thickness = Convert.ToDouble(thicknessCMB.Text) / 1000;    
            if (width <= 0 || thickness <= 0) return;  
            double singleVolume = logModel.Length * width * thickness; 
            if (singleVolume <= 0) return; 
            resultQuantity = Convert.ToInt32(Math.Floor(resultVolume / singleVolume)); 
            resultQuantityTextBox.Text = resultQuantity.ToString();  
            resultVolumeTextBox.Text = Math.Round(resultVolume,2).ToString();
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
             }
        }

        private void thicknessCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            okButton.Visibility = Visibility.Visible;
            
        }

        private void widthCMB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (widthCMB.Text !="" || thicknessCMB.Text != "")
            {
                okButton.Visibility = Visibility.Visible;
            }
        }
    }
}
