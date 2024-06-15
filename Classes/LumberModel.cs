using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DBApp
{
    internal class LumberModel : INotifyPropertyChanged
    { 
        private int id;
        private string type;
        private string grade;
        private double width;
        private double thickness;
        private double length;
        private double wetness;
        private int quantity;
        private double volume;
        private bool cut;
        private string dimensions;
        public double volumelog {  get; set; }
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

        public double Width
        {
            get => width;
            set
            {
                width = value;
                OnPropertyChanged(nameof(Width));

            }
        }

        public double Thickness
        {
            get => thickness;
            set
            {
                thickness = value;
                OnPropertyChanged(nameof(Thickness));
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
                OnPropertyChanged(nameof(Size));
            }
        }

        public double Volume
        {
            get => volume;
            set
            {
                volume = value;
                OnPropertyChanged(nameof(Volume));
                OnPropertyChanged(nameof(Size));

            }
        }

        public string Dimensions 
        {
            get {  
                return $"{Thickness}x{Width}";
            }
            set
            {
                dimensions = value;
                OnPropertyChanged(nameof(Dimensions));
               
            }
        }

        public bool Cut
        {
            get => cut;
            set
            {
                cut = value;
                OnPropertyChanged(nameof(Cut));
            }
        }

        public double Size
        {
            get
            {
                if (quantity != 0)
                    return volume / quantity;
                else
                    return 0;
            }
        }

    }
}