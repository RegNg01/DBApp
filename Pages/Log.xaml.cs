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
    /// Логика взаимодействия для Log.xaml
    /// </summary>
    public partial class Log : Page
    {
        SQLAuth auth;
        ObservableCollection<LogModel> LogModels;
        public Log()
        {
            InitializeComponent();
            auth = new SQLAuth();
            LoadData();  
        }
        public void LoadData()
        {
            try { 
            LogModels = new ObservableCollection<LogModel>();
            auth.connection.Open();
            try
            {
                string sql = "SELECT ws.ID, t.nameOfTheTreeType " +
                            "AS TypeName, ws.diameter1,ws.diameter2, ws.length, " +
                            "ws.quantity, tv.name as GradeName, ws.wetness, ws.volume " +
                             "FROM WoodStorage ws " +
                             "INNER JOIN TypeOfTree t ON ws.type = t.id " +
                             "INNER JOIN TreeVariety tv on ws.grade = tv.id";
                MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    LogModels.Add(new LogModel
                    {
                        ID = rdr.GetInt32("ID"),
                        Type = rdr.GetString("TypeName"),
                        Diameter1 = rdr.GetDouble("diameter1"),
                        Diameter2 = rdr.IsDBNull(rdr.GetOrdinal("diameter2")) ? 0 : rdr.GetDouble("diameter2"),
                        Length = rdr.GetDouble("length"),
                        Quantity = rdr.GetInt32("quantity"),
                        Grade = rdr.GetString("GradeName"),
                        Volume = rdr.GetDouble("volume"),
                        Wetness = rdr.GetDouble("wetness")
                    });
                }
                rdr.Close();
                   
                }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
               
                }
            logListView.ItemsSource = LogModels;
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

        }
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            LogModel selectedLog = button.DataContext as LogModel;
            EditLog editLog = new EditLog();
            editLog.DataContext = selectedLog;
            editLog.ShowDialog();
            LoadData();
            searchTextBox.Text = "";
            button.IsEnabled = true;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            LogModel selectedLog = button.DataContext as LogModel;
            try 
            {   
            auth.connection.Open();
            string sql = "DELETE FROM  WoodStorage WHERE id = @id";
            MySqlCommand cmd = new MySqlCommand(sql, auth.connection);
            cmd.Parameters.AddWithValue("@id", selectedLog.ID);
            cmd.ExecuteNonQuery();
            LogModels.Remove(selectedLog);
            auth.connection.Close();
            MessageBox.Show("Данные удалены"); 
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            } 
            button.IsEnabled = true; 
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        { 
            addButton.IsEnabled = false; 
            EditLog editLog = new EditLog();
            editLog.saveButton.Visibility = Visibility.Hidden;
            editLog.addButton.Visibility = Visibility.Visible;
            LogModel logModel = new LogModel();
            editLog.DataContext = logModel;
            editLog.ShowDialog();
            LoadData();
            searchTextBox.Text = "";
            addButton.IsEnabled = true;
        }

        private void sawButton_Click(object sender, RoutedEventArgs e)
        {
            LogModel selectedLog = logListView.SelectedItem as LogModel;
            if (selectedLog != null)
            {
                EditSaw editSaw = new EditSaw();
                editSaw.DataContext = selectedLog;
                editSaw.ShowDialog();
                LoadData();
                searchTextBox.Text = "";
            }
            else
            {
                MessageBox.Show("Выберите элемент для распила.");
            }
        }
         
        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();
            var filteredLogModels = LogModels
                .Where(log =>
                    log.ID.ToString().Contains(searchText) ||
                    log.Type.ToLower().Contains(searchText) ||
                    log.Diameter1.ToString().Contains(searchText) ||
                    log.Diameter2.ToString().Contains(searchText) ||
                    log.Length.ToString().Contains(searchText) ||
                    log.Quantity.ToString().Contains(searchText) ||
                    log.Grade.ToLower().Contains(searchText) ||
                    log.Wetness.ToString().Contains(searchText) ||
                    log.Volume.ToString().Contains(searchText))
                .ToList();
            logListView.ItemsSource = new ObservableCollection<LogModel>(filteredLogModels);
        }
    }
}
