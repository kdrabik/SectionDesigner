using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Majstersztyk
{
    public class TS_bar:INotifyPropertyChanged
    {
        private TS_point _Coordinates;

        public TS_point Coordinates {
            get { return _Coordinates; }
            set {
                _Coordinates = value;
                OnPropertyChanged("Coordinates");
            }
        }

        private double _Diameter;

        public double Diameter {
            get { return _Diameter; }
            set { _Diameter = value;
                OnPropertyChanged("Diameter");
            } }

		public double Area { get { return CalcArea(); }}
		//public TS_materials.TS_steel_rf SteelClass { get; set; }

        public TS_bar(TS_point coordinates, double diameter)//, TS_materials.TS_steel_rf steel)
        {
            this.Coordinates = coordinates;
            Diameter = diameter;
			//SteelClass = steel;
        }

        private double CalcArea()
        {
            return Math.Pow(Diameter / 2, 2) * Math.PI;
        }

        #region InotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
