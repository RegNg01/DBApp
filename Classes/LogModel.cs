using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DBApp
    {
        internal class LogModel : INotifyPropertyChanged
        {
            private int id;
            private string type;
            private string grade;
            private double diameter1;
            private double diameter2;
            private double length;
            private double wetness;
            private int quantity;
            private double volume; 
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public int ID
            {
                get => id;
                set
                {
                    id = value;
                    OnPropertyChanged(nameof(ID));
                }
            }

            public string Type
            {
                get => type;
                set
                {
                    type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }

            public string Grade
            {
                get => grade;
                set
                {
                    grade = value;
                    OnPropertyChanged(nameof(Grade));
                }
            }

        public double Diameter1
        {
            get => diameter1;
            set
            {
                diameter1 = value;
                OnPropertyChanged(nameof(Diameter1));
                OnPropertyChanged(nameof(DiameterRange));
            }
        }

        public double Diameter2
        {
            get => diameter2;
            set
            {
                diameter2 = value;
                OnPropertyChanged(nameof(Diameter2));
                OnPropertyChanged(nameof(DiameterRange));
            }
        }

        public string DiameterRange
        {
            get => diameter2 > 0 ? $"{diameter1}...{diameter2}" : diameter1.ToString();
            set
            {
                var diameters = value.Split(new string[] { "..." }, StringSplitOptions.None);
                if (diameters.Length == 2 &&
                    double.TryParse(diameters[0], out double d1) &&
                    double.TryParse(diameters[1], out double d2))
                {
                    Diameter1 = d1;
                    Diameter2 = d2;
                }
                else if (diameters.Length == 1 && double.TryParse(diameters[0], out double d))
                {
                    Diameter1 = d;
                    Diameter2 = 0; 
                }
            }
        }

        public double Length
            {
                get => length;
                set
                {
                    length = value;
                    OnPropertyChanged(nameof(Length));
                }
            }

            public double Wetness
            {
                get => wetness;
                set
                {
                    wetness = value;
                    OnPropertyChanged(nameof(Wetness));
                }
            }

            public int Quantity
            {
                get => quantity;
                set
                {
                    quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }

            public double Volume
            {
                get => volume;
                set
                {
                    volume = value;
                    OnPropertyChanged(nameof(Volume));
                }
            }
         
            public double Size
            {
            get
            {
                if (quantity!=0)
                return (volume / quantity);
                else return 0;
            }
            set
            {
                Size = value;
                OnPropertyChanged(nameof(Size));
            }
        }
       
        
    }
}
