using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Majstersztyk
{
    public class TS_bar:INotifyPropertyChanged
    {
        private TS_point _Coordinates;

        public TS_point Coordinates {
            get { return _Coordinates; }
            set {
                if (_Coordinates != null)
                    _Coordinates.PropertyChanged -= Bar_PropertyChanged;

                _Coordinates = value;

                if (_Coordinates != null) {
                    _Coordinates.PropertyChanged += Bar_PropertyChanged;
                }
                OnPropertyChanged();
            }
        }

        private double _Diameter;

        public double Diameter {
            get { return _Diameter; }
            set { _Diameter = value;
                OnPropertyChanged();
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

        private void OnPropertyChanged([CallerMemberName] string propertyName="") {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        void Bar_PropertyChanged(object sender, PropertyChangedEventArgs args) {
            OnPropertyChanged();
        }
        #endregion

    }
}
