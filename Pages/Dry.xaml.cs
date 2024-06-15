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
    /// Логика взаимодействия для Dry.xaml
    /// </summary>
    public partial class Dry : Page
    {
        SQLAuth auth;
        ObservableCollection<LumberModel> LumberModels;
        ObservableCollection<LumberModel> CutLumberModels;
        public Dry()
        {
            InitializeComponent();
            auth = new SQLAuth();
            LoadData();
        }
        public void LoadData()
        {
            try {  
            auth.connection.Open();
            LumberModels = new ObservableCollection<LumberModel>();
            CutLumberModels = new ObservableCollection<LumberModel>(); 
             
                string sql = "SELECT ds.ID, t.nameOfTheTreeType " +
                "AS TypeName, ds.width, ds.length, ds.thickness, " +
                "ds.quantity, tv.name as GradeName, ds.wetness, ds.volume, ds.cut " +
                "FROM DryStorage ds " +
                "INNER JOIN TypeOfTree t ON ds.type = t.id " +
                "INNER JOIN TreeVariety tv on ds.grade = tv.id"; 
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
            lumberListView.ItemsSource = LumberModels;
            cutLumberListView.ItemsSource = CutLumberModels;

             
        }
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            LumberModel selected = button.DataContext as LumberModel;
            EditDry editDry = new EditDry();
            editDry.startWetness = selected.Wetness;
            if (selected.Wetness > 15)
            {
                editDry.endWetness = 15;
                editDry.endWetnessTextBox.Text = "15";
                editDry.endVolume = selected.Volume * 0.9;
                editDry.endVolumeTextBox.Text = (selected.Volume * 0.9).ToString();
            }
            else
            {
                editDry.endWetness = selected.Wetness;
                editDry.endWetnessTextBox.Text = selected.Wetness.ToString();
                editDry.endVolume = selected.Volume;
                editDry.endVolumeTextBox.Text = selected.Volume.ToString();
            }
            editDry.startVolume = selected.Volume; 
            editDry.DataContext = selected;
            editDry.ShowDialog();
            button.IsEnabled = true;
            LoadData();
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
