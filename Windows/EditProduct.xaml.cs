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
    /// Логика взаимодействия для EditProduct.xaml
    /// </summary>
    public partial class EditProduct : Window
    {
        SQLAuth auth;
        public EditProduct()
        {
            InitializeComponent();
            auth = new SQLAuth();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            try {
                if (nameTextBox.Text.Trim() == "" ||
                        descriptionTextBox.Text.Trim() == "" ||
                        volumeTextBox.Text.Trim() == "" ||
                        quantityTextBox.Text.Trim() == "" ||
                         dimensionsTextBox.Text.Trim()==""|| 
                        typeCMB.SelectedIndex == -1 ||  
                        gradeCMB.SelectedIndex == -1 ||   
                        priceTextBox.Text.Trim() == "" 
                        )
                {
                    MessageBox.Show("Заполните все поля");
                    saveButton.IsEnabled = true;
                    return;
                }
                 
                auth.connection.Open();
                LumberModel lumberModel = this.DataContext as LumberModel;
                ProductModel productModel = new ProductModel();
                productModel.Grade = lumberModel.Grade;
                productModel.Type = lumberModel.Type;
                productModel.Volume = lumberModel.Volume;
                productModel.Quantity = lumberModel.Quantity;
                productModel.Length = lumberModel.Length;
                productModel.Thickness = lumberModel.Thickness;
                productModel.Width = lumberModel.Width;
                productModel.Dimensions = lumberModel.Dimensions;
                productModel.Name = nameTextBox.Text.Trim();
                productModel.Client = clientTextBox.Text.Trim();
                productModel.Description= descriptionTextBox.Text.Trim();
                productModel.Price = Convert.ToDouble(priceTextBox.Text.Trim());
               
                string sql = "INSERT INTO ProductStorage " +
                "(type, grade, width, thickness, length, quantity, volume, name, description, price, client) " +
                "VALUES ((SELECT id FROM TypeOfTree WHERE nameOfTheTreeType = @type), " +
                "(SELECT id FROM TreeVariety WHERE name = @grade), " +
                "@width, @thickness, @length, @quantity, @volume, @name, @description, @price, @client)";
            MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
            
            cmd.Parameters.AddWithValue("@type", productModel.Type);
            cmd.Parameters.AddWithValue("@grade", productModel.Grade); 
            cmd.Parameters.AddWithValue("@width", productModel.Width);
            cmd.Parameters.AddWithValue("@thickness", productModel.Thickness);
            cmd.Parameters.AddWithValue("@length", productModel.Length);
            cmd.Parameters.AddWithValue("@volume", productModel.Volume);
            cmd.Parameters.AddWithValue("@quantity", productModel.Quantity);
            cmd.Parameters.AddWithValue("@name", productModel.Name);
            cmd.Parameters.AddWithValue("@description", productModel.Description);
            cmd.Parameters.AddWithValue("@price", productModel.Price);
            cmd.Parameters.AddWithValue("@client", productModel.Client);  
            cmd.ExecuteNonQuery();
                string sql1 = "DELETE FROM LumberStorage WHERE id = @id";
                MySqlCommand cmd1 = new MySqlCommand(sql1, auth.connection); 
                cmd1.Parameters.AddWithValue("@id", lumberModel.ID);
                cmd1.ExecuteNonQuery();
                MessageBox.Show("Данные успешно сохранены");
            auth.connection.Close(); 
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (auth.connection.State == System.Data.ConnectionState.Open)
                {
                    auth.connection.Close();
                }
            }
            this.Close();

        }
         
         
    }
}
