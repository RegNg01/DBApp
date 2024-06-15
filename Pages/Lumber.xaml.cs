using DBApp.Windows;
using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
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
    /// Логика взаимодействия для Lumber.xaml
    /// </summary>
    public partial class Lumber : Page
    {
        SQLAuth auth;
        ObservableCollection<LumberModel> LumberModels;
        ObservableCollection<LumberModel> CutLumberModels;
        public Lumber()
        {
            
            InitializeComponent();
            auth = new SQLAuth();
            LoadData();

        }
        public void LoadData()
        {
             
            LumberModels = new ObservableCollection<LumberModel>();
            CutLumberModels = new ObservableCollection<LumberModel>();
             
            try
            {
                auth.connection.Open();
                string sql = "SELECT ls.ID, t.nameOfTheTreeType AS TypeName, " +
                "ls.width, ls.length, ls.thickness, ls.quantity, tv.name as GradeName, " +
                "ls.wetness, ls.volume, ls.cut " +
                "FROM LumberStorage ls " +
                "INNER JOIN TypeOfTree t ON ls.type = t.id " +
                "INNER JOIN TreeVariety tv on ls.grade = tv.id";

                MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var lumberModel = new LumberModel
                    {
                        ID = rdr.GetInt32("ID"),
                        Type = rdr.GetString("TypeName"),
                        Width = rdr.GetDouble("width"),
                        Thickness = rdr.GetDouble("thickness"),
                        Length = rdr.GetDouble("length"),
                        Quantity = rdr.GetInt32("quantity"),
                        Grade = rdr.GetString("GradeName"),
                        Volume = rdr.GetDouble("volume"),
                        Wetness = rdr.GetDouble("wetness"),
                        Cut = rdr.GetBoolean("cut")
                    };

                    if (lumberModel.Cut)
                    {
                        CutLumberModels.Add(lumberModel);
                    }
                    else
                    {
                        LumberModels.Add(lumberModel);
                    }
                }

                rdr.Close();
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
            lumberListView.ItemsSource = LumberModels;   
             cutLumberListView.ItemsSource = CutLumberModels;  

           
        }
        private void shaveButton_Click(object sender, RoutedEventArgs e)
        {
            try {  
            auth.connection.Open();
            LumberModel selectedmodel = lumberListView.SelectedItem as LumberModel;
            if (selectedmodel!= null)
            {
                try
                {
                    string sql = "UPDATE `LumberStorage` SET `cut` = '1', " +
                        "volume = @volume WHERE `LumberStorage`.`id` = @id";

                    MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
                    cmd.Parameters.AddWithValue("@id", selectedmodel.ID);
                    cmd.Parameters.AddWithValue("@volume", selectedmodel.Volume*0.85);
                    cmd.ExecuteNonQuery();
                        
                    }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            else
            {
                MessageBox.Show("Выберите элемент для обрезки.");
            }
                auth.connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message );
            }
            finally
            {
                if (auth.connection.State == System.Data.ConnectionState.Open)
                {
                    auth.connection.Close();
                }
            }
            LoadData();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            LumberModel selected = button.DataContext as LumberModel;
            auth.connection.Open();
            string sql = "DELETE FROM  LumberStorage WHERE id = @id";
            MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
            cmd.Parameters.AddWithValue("@id", selected.ID);
            cmd.ExecuteNonQuery();
            auth.connection.Close();
            LoadData(); 
            button.IsEnabled = true;
            MessageBox.Show("Данные удалены");
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            LumberModel selected = button.DataContext as LumberModel;
            EditLumber editLumber = new EditLumber();
            editLumber.DataContext = selected;
            if (selected.Cut)
                editLumber.cutTextBlock.Text = "Обрезная";
            else
                editLumber.cutTextBlock.Text = "Необрезная";
            editLumber.ShowDialog();
            LoadData();
            button.IsEnabled = true;
        }

       

      
        private void dryButton_Click(object sender, RoutedEventArgs e)
        {

            LumberModel selected = lumberListView.SelectedItem as LumberModel;
            LumberModel selected1 = cutLumberListView.SelectedItem as LumberModel;
            if (selected == null && selected1 == null)
                MessageBox.Show("Выберите элемент для сушки");
            else
                if (selected1 == null)
            {
                EditToDry editToDry = new EditToDry();
                editToDry.DataContext = selected;
                editToDry.startQuantity = selected.Quantity;
                editToDry.endQuantity = selected.Quantity;
                editToDry.startVolume = selected.Volume;
                editToDry.endVolume = selected.Volume;
                editToDry.sizeVolume = selected.Size;
                editToDry.ShowDialog();
                LoadData();
            }
            else if (selected == null)
              {
                EditToDry editToDry = new EditToDry();
                editToDry.DataContext = selected1;
                editToDry.startQuantity = selected1.Quantity;
                editToDry.endQuantity = selected1.Quantity;
                editToDry.startVolume = selected1.Volume;
                editToDry.endVolume = selected1.Volume;
                editToDry.sizeVolume = selected1.Size;
                editToDry.ShowDialog();
                LoadData();
            }

           


        }
        private void lumberListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lumberListView.SelectedItem != null)
            {
                cutLumberListView.SelectedItem = null;
            }
        }

        private void cutLumberListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cutLumberListView.SelectedItem != null)
            {
                lumberListView.SelectedItem = null;
            }
        }
        private void productButton_Click(object sender, RoutedEventArgs e)
        {
            
            LumberModel selected = cutLumberListView.SelectedItem as LumberModel;
            EditProduct editProduct = new EditProduct();
            editProduct.DataContext = selected;
            editProduct.ShowDialog();
            LoadData();
            
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            addButton.IsEnabled = false;
            EditLumber editLumber = new EditLumber();
            editLumber.saveButton.Visibility = Visibility.Hidden;
            editLumber.addButton.Visibility = Visibility.Visible;
            LumberModel lumberModel = new LumberModel();
            editLumber.DataContext = lumberModel;
            editLumber.ShowDialog();
            LoadData();
            searchTextBox.Text = "";
            addButton.IsEnabled = true;
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();

            var filteredLumberModels = LumberModels
                .Where(lumber =>
                    lumber.ID.ToString().Contains(searchText) ||
                    lumber.Type.ToLower().Contains(searchText) ||
                    lumber.Dimensions.ToString().Contains(searchText) ||
                    lumber.Length.ToString().Contains(searchText) ||
                    lumber.Quantity.ToString().Contains(searchText) ||
                    lumber.Grade.ToLower().Contains(searchText) ||
                    lumber.Wetness.ToString().Contains(searchText) ||
                    lumber.Volume.ToString().Contains(searchText))
                .ToList();

            var filteredCutLumberModels = CutLumberModels
                .Where(lumber =>
                    lumber.ID.ToString().Contains(searchText) ||
                    lumber.Type.ToLower().Contains(searchText) ||
                    lumber.Dimensions.ToString().Contains(searchText) ||
                    lumber.Length.ToString().Contains(searchText) ||
                    lumber.Quantity.ToString().Contains(searchText) ||
                    lumber.Grade.ToLower().Contains(searchText) ||
                    lumber.Wetness.ToString().Contains(searchText) ||
                    lumber.Volume.ToString().Contains(searchText))
                .ToList();

            lumberListView.ItemsSource = new ObservableCollection<LumberModel>(filteredLumberModels);
            cutLumberListView.ItemsSource = new ObservableCollection<LumberModel>(filteredCutLumberModels);

        }
    }
}
