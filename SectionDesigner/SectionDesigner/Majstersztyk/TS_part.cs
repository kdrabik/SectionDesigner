/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 07/12/2018
 * Time: 11:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SectionDesigner;

namespace Majstersztyk
{
    /// <summary>
    /// Description of TS_ordinate.
    /// </summary>s
    /// 

    public class TS_part : TS_region, INotifyPropertyChanged
    {
        private ObservableList<TS_void> _Voids;

        public ObservableList<TS_void> Voids {
            get { return _Voids; }
            set {
                if (_Voids != null)
                    _Voids.PropertyChanged -= Part_OnPropertyChanged;

                _Voids = value;

                if (_Voids != null) {
                    _Voids.PropertyChanged += Part_OnPropertyChanged;
                }
                OnPropertyChanged();
            }
        }

        private TS_contour _Contour;

        public TS_contour Contour {
            get { return _Contour; }
            set {
                if (_Contour != null)
                    _Contour.PropertyChanged -= Part_OnPropertyChanged;

                _Contour = value;

                if (_Contour != null) {
                    _Contour.PropertyChanged += Part_OnPropertyChanged;
                }
                OnPropertyChanged();
            }
        }

        private TS_materials.TS_material _Material;

        public TS_materials.TS_material Material {
            get { return _Material; }
            set {
                if (_Material != null)
                    _Material.PropertyChanged -= Part_OnPropertyChanged;

                _Material = value;

                if (_Material != null) {
                    _Material.PropertyChanged += Part_OnPropertyChanged;
                }
                OnPropertyChanged();
            }
        }

        private ObservableList<TS_contour> _GeometryComponents;

        public ObservableList<TS_contour> GeometryComponents{
            get { return _GeometryComponents; }
            private set {
                ObservableList<TS_contour> geomComp = new ObservableList<TS_contour>();
                geomComp.Add(_Contour);
                geomComp.AddRange(_Voids);
                _GeometryComponents = geomComp;
                OnPropertyChanged();
            }
        }

        public override string TypeOf { get { return typeOf; } }
        private new readonly string typeOf = "Part";

        public TS_part(TS_materials.TS_material material, TS_contour contour, List<TS_void> voids)
        {
			Material = material;
            Voids = new ObservableList<TS_void>();
            Contour = contour;
            Voids.AddRange(voids);
            GeometryComponents = null;
        }

        #region Calculation
        protected override double CalcArea()
        {
            double area = Contour.Area;
            foreach (var myVoid in Voids) {
				area += myVoid.Area;
            }
			return area;
        }

        protected override double CalcSx(){
			double sx = Contour.StaticMomX;
			foreach (var myVoid in Voids) {
				sx += myVoid.StaticMomX;
            }
			return sx;
        }

        protected override double CalcSy(){
        	double sy = Contour.StaticMomY;
			foreach (var myVoid in Voids) {
				sy += myVoid.StaticMomY;
            }
			return sy;
        }

        protected override double CalcIx(){
			double ix = Contour.InertiaMomX;
			foreach (var myVoid in Voids) {
				ix += myVoid.InertiaMomX;
            }
			return ix;        	
        }

        protected override double CalcIy(){
			double iy = Contour.InertiaMomY;
			foreach (var myVoid in Voids) {
				iy += myVoid.InertiaMomY;
            }
			return iy;   
        }

        protected override  double CalcIxy(){
 			double ixy = Contour.DeviationMomXY;
			foreach (var myVoid in Voids) {
				ixy += myVoid.DeviationMomXY;
            }
			return ixy; 
        }
        #endregion

        #region Helps
        protected override bool IsObjectCorrect()
        {
            if (!Contour.IsCorrect)
                return false;

            foreach (var Void in Voids)
            {
                if (!Void.IsCorrect)
                    return false;
            }

            foreach (var Void in Voids) {
                foreach (var vert in Void.Vertices) {
                    if (!Contour.IsPointInside(vert))
                        return false;
                }
            }

            for (int i = 0; i < Voids.Count; i++) {
                for (int j = 0; j < Voids.Count; j++) {
                    if (i != j) {
                        foreach (var vert in Voids[i].Vertices) {
                            if (Voids[j].IsPointInside(vert)) return false;
                        }
                    }
                }
            }

            return true;
        }
        
		public override string ToString()
		{
			string text = "";
			text += Environment.NewLine + Environment.NewLine + "Material: " + Material.Name 
				+ " Elastic modulus: " + String.Format("{0:e2}", Material.E);
			text += base.ToString();
			text += Contour.ToString();
			
			foreach (var tenvoid in Voids) {
				text += tenvoid.ToString();
			}
			return text;
		}
        #endregion

        protected void Part_OnPropertyChanged(object sender, PropertyChangedEventArgs args) {
            if (_Material != null && _Voids != null && _Contour != null) {
                CalcProperties();
                OnPropertyChanged();
            }
        }
    }
}
