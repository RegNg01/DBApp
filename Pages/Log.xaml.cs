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
        
        public Log()
        {
            InitializeComponent();
            ObservableCollection<LogModel> LogModels = new ObservableCollection<LogModel>();
            Random random = new Random();
            for (int i = 1; i <= 10; i++)
            {
                LogModels.Add(new LogModel
                {
                    ID = i,
                    Type = "Дуб",
                    Diameter = random.Next(30,71),
                    Length = random.Next(200, 501),
                    Quantity = random.Next(5, 36)
                });
            }
              logListView.ItemsSource = LogModels;
            
        }
    }
}
