using DBApp.Pages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;

namespace DBApp
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        SQLAuth auth;
        public UserWindow()
        {
            InitializeComponent();
            auth = new SQLAuth();
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


        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void navBarListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (navBarListView.SelectedItem != null)
            {
                switch (Convert.ToInt32(navBarListView.SelectedValue))
                {
                    case 1:
                        navFrame.Content = new Log();
                        break;
                    case 2:
                        navFrame.Content = new Saw();
                        break;
                    case 3:
                        navFrame.Content = new Lumber();
                        break;
                    case 4:
                        navFrame.Content = new Dry();
                        break;
                    case 5:
                        navFrame.Content = new Product();
                        break;
                    default:
                        break;
                }
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }


        private void GenerateReport()
        {
            try
            {
                StringBuilder report = new StringBuilder();
                auth.connection.Open();

                List<string> tables = new List<string> { "WoodStorage", "LumberStorage", "SawStorage", "ProductStorage", "DryStorage", "Role", "LoginDetails", "TreeVariety", "TypeOfTree" };
                report.AppendLine("Отчет о состоянии таблиц и их связях в базе данных:");

                foreach (var table in tables)
                {
                    report.AppendLine($"\nТаблица: {table}");
                    string sqlForeignKeys = $@"
                        SELECT 
                            COLUMN_NAME, 
                            REFERENCED_TABLE_NAME, 
                            REFERENCED_COLUMN_NAME 
                        FROM 
                            information_schema.KEY_COLUMN_USAGE 
                        WHERE 
                            TABLE_SCHEMA = '{auth.connection.Database}' AND 
                            TABLE_NAME = '{table}' AND 
                            REFERENCED_TABLE_NAME IS NOT NULL";

                    MySqlCommand cmdForeignKeys = new MySqlCommand(sqlForeignKeys, auth.connection);
                    MySqlDataReader readerForeignKeys = cmdForeignKeys.ExecuteReader();

                    if (readerForeignKeys.HasRows)
                    {
                        report.AppendLine("Связи:");
                        while (readerForeignKeys.Read())
                        {
                            string columnName = readerForeignKeys.GetString("COLUMN_NAME");
                            string referencedTableName = readerForeignKeys.GetString("REFERENCED_TABLE_NAME");
                            string referencedColumnName = readerForeignKeys.GetString("REFERENCED_COLUMN_NAME");

                            report.AppendLine($"- {columnName} -> {referencedTableName}({referencedColumnName})");
                        }
                    }
                    else
                    {
                        report.AppendLine("Нет внешних ключей.");
                    }
                    readerForeignKeys.Close();
                }

                auth.connection.Close();
                MessageBox.Show(report.ToString());
              
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    FlowDocument doc = new FlowDocument(new Paragraph(new Run(report.ToString())));
                    doc.Name = "Report";
                    printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Отчет о таблицах и их связях");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при генерации отчета: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateReport();
        }
    }
}
