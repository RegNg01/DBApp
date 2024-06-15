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
    /// Логика взаимодействия для Saw.xaml
    /// </summary>
    public partial class Saw : Page
    {
        SQLAuth auth;
        ObservableCollection<LumberModel> LumberModels;
        public Saw()
        {
            InitializeComponent();
            auth = new SQLAuth(); 
            LoadData();
        }
        public void LoadData()
        {
            
            LumberModels = new ObservableCollection<LumberModel>();
            try
            {
                auth.connection.Open();
                string sql = "SELECT sw.ID, t.nameOfTheTreeType " +
                "AS TypeName, sw.width, sw.length, sw.volumelog, sw.thickness, sw.quantity, " +
                "tv.name as GradeName, sw.wetness, sw.volume, sw.cut " +
                "FROM SawStorage sw " +
                "INNER JOIN TypeOfTree t ON sw.type = t.id " +
                "INNER JOIN TreeVariety tv on sw.grade = tv.id";

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
                        Cut = rdr.GetBoolean("cut"),
                        volumelog= rdr.GetDouble("volumelog")
                    };
                    

                    LumberModels.Add(lumberModel);
                }

                rdr.Close();
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
            sawListView.ItemsSource = LumberModels;
             
           
        }
       
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false; 
            try
            {
                auth.connection.Open();
                LumberModel selected = button.DataContext as LumberModel;
               
                string sql = "INSERT INTO `LumberStorage` " +
                    "(`type`, `grade`, `width`, `thickness`, `length`, " +
                    "`wetness`, `quantity`, `volume`, `cut`) " +
                    "VALUES ((SELECT id FROM TypeOfTree WHERE nameOfTheTreeType = " +
                    "@type), (SELECT id FROM TreeVariety WHERE name = @grade), @width, " +
                    "@thickness, @length, @wetness, " +
                    "@quantity, @volume, @cut)";
                MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
                cmd.Parameters.AddWithValue("@type", selected.Type);
                cmd.Parameters.AddWithValue("@grade", selected.Grade);
                cmd.Parameters.AddWithValue("@width", selected.Width);
                cmd.Parameters.AddWithValue("@thickness", selected.Thickness);
                cmd.Parameters.AddWithValue("@length", selected.Length);
                cmd.Parameters.AddWithValue("@wetness", selected.Wetness);
                cmd.Parameters.AddWithValue("@quantity", selected.Quantity);
                cmd.Parameters.AddWithValue("@volume", selected.Volume);
                cmd.Parameters.AddWithValue("@cut", selected.Cut);
             
                cmd.ExecuteNonQuery();

                string sql1 = "DELETE FROM SawStorage WHERE SawStorage.id = @id";
                MySqlCommand cmd1 = new MySqlCommand(sql1, auth.connection);
                cmd1.Parameters.AddWithValue("@id", selected.ID);
                cmd1.ExecuteNonQuery(); 
                MessageBox.Show("Данные успешно сохранены");
               
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
                    LoadData();
                    searchTextBox.Text = "";
                }
            }
            button.IsEnabled = true;
            
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();
            var filteredLumberModels = LumberModels
                .Where(lumber =>
                    lumber.ID.ToString().Contains(searchText) ||
                    lumber.Type.ToLower().Contains(searchText) ||
                    lumber.volumelog.ToString().Contains(searchText) ||
                    lumber.Dimensions.ToString().Contains(searchText) || 
                    lumber.Length.ToString().Contains(searchText) ||
                   lumber.Quantity.ToString().Contains(searchText) ||
                    lumber.Grade.ToLower().Contains(searchText) ||
                    lumber.Wetness.ToString().Contains(searchText) ||
                   lumber.Volume.ToString().Contains(searchText))
                .ToList();
           sawListView.ItemsSource = new ObservableCollection<LumberModel>(filteredLumberModels);
        }
    }
}
