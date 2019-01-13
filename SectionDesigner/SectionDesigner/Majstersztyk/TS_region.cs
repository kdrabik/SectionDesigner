using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using SectionDesigner;

namespace Majstersztyk
{
	public abstract class TS_region : INotifyParametersChanged
    {
        #region Properties
        public bool IsCorrect { 
        	get { return IsObjectCorrect(); } }

        private string _Name;

        public string Name {
            get { return _Name; }
            set { _Name = value;
                OnPropertyChanged();
            }
        }
        private double _Area; 
        public virtual double Area {
            get { return _Area; }
            protected set { _Area = value; OnPropertyChanged(); } }

        private double _StaticMomX;
        public virtual double StaticMomX {
            get { return _StaticMomX; }
            protected set { _StaticMomX = value; OnPropertyChanged(); } }

        private double _StaticMomY;
        public virtual double StaticMomY {
            get { return _StaticMomY; }
            protected set { _StaticMomY = value; OnPropertyChanged(); }
        }

        private double _InertiaMomX;
        public virtual double InertiaMomX {
            get { return _InertiaMomX; }
            protected set { _InertiaMomX = value; OnPropertyChanged(); }
        }

        private double _InertiaMomY;
        public virtual double InertiaMomY {
            get { return _InertiaMomY; }
            protected set { _InertiaMomY = value; OnPropertyChanged(); }
        }

        private double _DeviationMomXY;
        public virtual double DeviationMomXY {
            get { return _DeviationMomXY; }
            protected set { _DeviationMomXY = value; OnPropertyChanged(); } }

        private TS_point _Centroid;
        public virtual TS_point Centroid {
            get { return _Centroid; }
            protected set { _Centroid = value; OnPropertyChanged(); } }

        private double _CentrInertiaMom_X;
        public virtual double CentrInertiaMom_X {
            get { return _CentrInertiaMom_X; }
            protected set { _CentrInertiaMom_X = value; OnPropertyChanged(); } }

        private double _CentrInertiaMom_Y;
        public virtual double CentrInertiaMom_Y {
            get { return _CentrInertiaMom_Y; }
            protected set { _CentrInertiaMom_Y = value; OnPropertyChanged(); }
        }

        private double _CentrDeviationMom_XY;
        public virtual double CentrDeviationMom_XY {
            get { return _CentrDeviationMom_XY; }
            protected set { _CentrDeviationMom_XY = value; OnPropertyChanged(); }
        }

        private double _CentrPrincipleInertiaMom_1;
        public virtual double CentrPrincipleInertiaMom_1 {
            get { return _CentrPrincipleInertiaMom_1; }
            protected set { _CentrPrincipleInertiaMom_1 = value; OnPropertyChanged(); }
        }

        private double _CentrPrincipleInertiaMom_2;
        public virtual double CentrPrincipleInertiaMom_2 {
            get { return _CentrPrincipleInertiaMom_2; }
            protected set { _CentrPrincipleInertiaMom_2 = value; OnPropertyChanged(); }
        }

        private double _AngleOfPrincipleLayout;
        public virtual double AngleOfPrincipleLayout {
            get { return _AngleOfPrincipleLayout; }
            protected set { _AngleOfPrincipleLayout = value; OnPropertyChanged(); }
        }
        
        public virtual string TypeOf {get{return typeOf;}}
		protected string typeOf;
      
        protected TS_region _SelectedMember;
        public TS_region SelectedMember{
        	get{ return _SelectedMember;}
        	set{
                _SelectedMember = value;
        		OnSelectedMemberChanged();} }
		
        #endregion

        #region Calculation Methods
        protected abstract double CalcArea();
        protected abstract double CalcSx();
        protected abstract double CalcSy();
        protected abstract double CalcIx();
        protected abstract double CalcIy();
        protected abstract double CalcIxy();

        protected abstract bool IsObjectCorrect();
        //public abstract void Update();

        protected virtual void CalcProperties()
        {
            Area = CalcArea();
            StaticMomX = CalcSx();
            StaticMomY = CalcSy();
            InertiaMomX = CalcIx();
            InertiaMomY = CalcIy();
            DeviationMomXY = CalcIxy();
            Centroid = CalcCentroid();
            CalcCentralProp();
            CalcCentralMainProp();
            //Console.WriteLine(this.ToString());
        }
        
        protected TS_point CalcCentroid()
        {
            return new TS_point(StaticMomY / Area, StaticMomX / Area);
        }

        protected virtual void CalcCentralProp() {
            CentrInertiaMom_X = InertiaMomX - Math.Pow(Centroid.Y, 2) * Area;
            CentrInertiaMom_Y = InertiaMomY - Math.Pow(Centroid.X, 2) * Area;
            CentrDeviationMom_XY = DeviationMomXY - (Centroid.X * Centroid.Y) * Area;
        }

        private void CalcCentralMainProp() {
            if (AreDoublesEqual(CentrInertiaMom_X, CentrInertiaMom_Y)) {
                AngleOfPrincipleLayout = 0;
            } else {
                double tg2fi0 = -2 * CentrDeviationMom_XY / (CentrInertiaMom_X - CentrInertiaMom_Y);
                AngleOfPrincipleLayout = Math.Atan(tg2fi0) / 2;
            }
            CentrPrincipleInertiaMom_1 = 1d / 2 * (CentrInertiaMom_X + CentrInertiaMom_Y) +
                1d / 2 * Math.Sqrt(Math.Pow(CentrInertiaMom_X - CentrInertiaMom_Y, 2) + 4 * Math.Pow(CentrDeviationMom_XY, 2));
            CentrPrincipleInertiaMom_2 = 1d / 2 * (CentrInertiaMom_X + CentrInertiaMom_Y) -
            1d / 2 * Math.Sqrt(Math.Pow(CentrInertiaMom_X - CentrInertiaMom_Y, 2) + 4 * Math.Pow(CentrDeviationMom_XY, 2));
        }

        #endregion

        public static bool AreDoublesEqual(double d1, double d2) {
            double diff = Math.Abs(Math.Min(d1, d2) / 1000000);

            if (Math.Abs(d1 - d2) <= diff) return true;

            return false;
        }

        public override string ToString() {
            string text = "";
            string format = "{0:e4}";

            text += Environment.NewLine + "Name: " + Name + 
                Environment.NewLine + "Type: " + TypeOf + 
                Environment.NewLine + "Is correct?: " + IsCorrect;
            text += Environment.NewLine + "Area: " + String.Format(format, Area);
            text += Environment.NewLine + "Static moment X: " + String.Format(format, StaticMomX);
            text += Environment.NewLine + "Static moment Y: " + String.Format(format, StaticMomY);
            text += Environment.NewLine + "Inertia moment X: " + String.Format(format, InertiaMomX);
            text += Environment.NewLine + "Inertia moment Y: " + String.Format(format, InertiaMomY);
            text += Environment.NewLine + "Deviation moment XY: " + String.Format(format, DeviationMomXY);
            text += Environment.NewLine + "Centroid: " + Centroid.ToString();
            text += Environment.NewLine + "Central inertia moment X: " + String.Format(format, CentrInertiaMom_X);
            text += Environment.NewLine + "Central inertia moment Y: " + String.Format(format, CentrInertiaMom_Y);
            text += Environment.NewLine + "Central deviation moment XY: " + String.Format(format, CentrDeviationMom_XY);
            text += Environment.NewLine + "Central principle inertia moment X: " + String.Format(format, CentrPrincipleInertiaMom_1);
            text += Environment.NewLine + "Central principle inertia moment Y: " + String.Format(format, CentrPrincipleInertiaMom_2);
            text += Environment.NewLine + "Angle of principle layout: " + String.Format(format, AngleOfPrincipleLayout / Math.PI * 360) + " [deg]";

            return text;
        }
        
        #region InotifyPropertyChanged Members
	
		public event PropertyChangedEventHandler PropertyChanged;
			
		protected void OnPropertyChanged([CallerMemberName] string propertyName="") {
			PropertyChangedEventHandler handler = PropertyChanged;
			
			if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion

        #region InotifyParametersChanged Members
        public event EventHandler ParametersChanged;

        protected void OnParametersChanged() {
            EventHandler handler = ParametersChanged;

            if (handler != null) {
                handler(this, new EventArgs());
            }
        }
        #endregion
        
        public virtual void ContainedElementParametersChanged(object sender, EventArgs args) {
            CalcProperties();
			OnParametersChanged();
        }
        
        public event EventHandler SelectedMemberChanged;

        protected void OnSelectedMemberChanged() {
            EventHandler handler = SelectedMemberChanged;

            if (handler != null) {
                handler(this, new EventArgs());
            }
        }

    }
}
