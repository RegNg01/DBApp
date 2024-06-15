using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DBApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для Product.xaml
    /// </summary>
    public partial class Product : Page
    {
        SQLAuth auth;
        ObservableCollection<ProductModel> productModels;
        public Product()
        {
            InitializeComponent();
           auth = new SQLAuth();
           LoadData();
        }
        public void LoadData()
        {
            auth.connection.Open();
            productModels = new ObservableCollection<ProductModel>();
            
            try
            {
                string sql = "SELECT ps.id, ps.name, tv.name AS GradeName, " +
                "t.nameOfTheTreeType AS TypeName, ps.width, ps.thickness, ps.length, " +
                "ps.quantity, ps.volume, ps.description, ps.price, ps.client "+
                "FROM ProductStorage ps " +
                "INNER JOIN TypeOfTree t ON ps.type = t.id " +
                "INNER JOIN TreeVariety tv  ON ps.grade = tv.id"; 
                MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var productModel = new ProductModel
                    {
                        ID = rdr.GetInt32("ID"),
                        Type = rdr.GetString("TypeName"),
                        Name = rdr.GetString("name"),
                        Width = rdr.GetDouble("width"),
                        Thickness = rdr.GetDouble("thickness"),
                        Length = rdr.GetDouble("length"),
                        Quantity = rdr.GetInt32("quantity"),
                        Grade = rdr.GetString("GradeName"),
                        Volume = rdr.GetDouble("volume"),
                        Client = rdr.GetString("client"),
                        Description = rdr.GetString("description"), 
                        Price = rdr.GetDouble("price")
                    };
                     
                        productModels.Add(productModel);
                    
                }

                rdr.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            productListView.ItemsSource = productModels;
             

            auth.connection.Close();
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();
            var filteredProductModels = productModels
                .Where(log =>
                    log.ID.ToString().Contains(searchText) ||
                    log.Type.ToLower().Contains(searchText) ||
                    log.Name.ToLower().Contains(searchText) ||
                    log.Description.ToLower().Contains(searchText) ||
                    log.Client.ToLower().Contains(searchText) ||
                    log.Dimensions.ToString().Contains(searchText) || 
                    log.Length.ToString().Contains(searchText) ||
                    log.Quantity.ToString().Contains(searchText) ||
                    log.Grade.ToLower().Contains(searchText) ||
                    log.Price.ToString().Contains(searchText) ||
                    log.Volume.ToString().Contains(searchText))
                .ToList();
            productListView.ItemsSource = new ObservableCollection<ProductModel>(filteredProductModels);
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            ProductModel selected = button.DataContext as ProductModel;
            auth.connection.Open();
            string sql = "DELETE FROM  ProductStorage WHERE id = @id";
            MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
            cmd.Parameters.AddWithValue("@id", selected.ID);
            cmd.ExecuteNonQuery();
            auth.connection.Close();
            LoadData();
            button.IsEnabled = true;
            MessageBox.Show("Данные удалены");
        }
         
    }
}
