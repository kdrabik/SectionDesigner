using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Majstersztyk
{
    public class TS_reinforcement:TS_region
    {
        private List<TS_bar> _Bars;

        public List<TS_bar> Bars {
            get { return _Bars; }
            set { _Bars = value;
                OnPropertyChanged("Bars");
                CalcProperties();
            }
        }

        private TS_materials.TS_material _Material;

        public TS_materials.TS_material Material {
            get { return _Material; }
            set { _Material = value;
                OnPropertyChanged("Material");
                CalcProperties();
            }
        }

        public override string TypeOf { get { return typeOf; } }
        private new readonly string typeOf = "Reinforcement";

        public TS_reinforcement() {
            _Bars = new List<TS_bar>();
            _Material = null;
        }

        public TS_reinforcement(List<TS_bar> bars, TS_materials.TS_material material) {
            _Bars = bars;
            _Material = material;
            CalcProperties();
        }

        protected override double CalcArea(){
			double area = 0;
        	foreach (var bar in _Bars) {
				area += bar.Area;
        	}
			return area;
        }
        
        protected override double CalcSx(){
			double sx = 0;
			foreach (var bar in _Bars) {
				sx += bar.Area * bar.Coordinates.Y;
			}
			return sx;
        }
        
        protected override double CalcSy(){
        	double sy = 0;
			foreach (var bar in _Bars) {
				sy += bar.Area * bar.Coordinates.X;
			}
			return sy;
        }
        
        protected override double CalcIx(){
			double ix = 0;
			foreach (var bar in _Bars) {
				double ib = Math.PI * Math.Pow(bar.Diameter, 4) / 64;
				ix += ib + bar.Area * Math.Pow(bar.Coordinates.Y, 2);
			}
			return ix;
        }
        
        protected override double CalcIy(){
        	double iy = 0;
			foreach (var bar in _Bars) {
				double ib = Math.PI * Math.Pow(bar.Diameter, 4) / 64;
				iy += ib + bar.Area * Math.Pow(bar.Coordinates.X, 2);
			}
			return iy;
        }

        protected override double CalcIxy(){
			double ixy = 0;
			foreach (var bar in _Bars) {
				ixy += bar.Area * bar.Coordinates.X * bar.Coordinates.Y;
			}
			return ixy;
        }

        protected override void CalcCentralProp() {
            double centrx = InertiaMomX - Math.Pow(Centroid.Y, 2) * Area;
            double centry = InertiaMomY - Math.Pow(Centroid.X, 2) * Area;
            double centrxy = DeviationMomXY - (Centroid.X * Centroid.Y) * Area;
            
            CentrInertiaMom_X = centrx;
            CentrInertiaMom_Y = centry;
            CentrDeviationMom_XY = centrxy;
        }

        protected override bool IsObjectCorrect(){
        	for (int i = 0; i < _Bars.Count; i++) {
        		for (int j = 0; j < _Bars.Count; j++) {
        			if (i != j) {
        				double dist = Math.Sqrt(Math.Pow(_Bars[i].Coordinates.X-_Bars[j].Coordinates.X,2)+Math.Pow(_Bars[i].Coordinates.Y-_Bars[j].Coordinates.Y,2));
        				double minDist = 0.5 * (_Bars[i].Diameter + _Bars[j].Diameter);
        				if (dist < minDist) {
							return false;
        				}
        			}
        		}
        	}
			return true;
        }
        
        public override string ToString()
		{
			string text = "";
			text += Environment.NewLine + Environment.NewLine + "Material: " + _Material.Name 
				+ " Elastic modulus: " + String.Format("{0:E2}", _Material.E);
			text += base.ToString();
			
			return text;
		}
        
    }
}
