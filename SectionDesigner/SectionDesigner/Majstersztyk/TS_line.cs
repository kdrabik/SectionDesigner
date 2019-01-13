/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 05/12/2018
 * Time: 14:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Majstersztyk
{
	/// <summary>
	/// Description of TS_line.
	/// </summary>
	public class TS_line : INotifyPropertyChanged
    {
		#region dla postaci prostej Ax + By + C = 0
        private double _GenA;

        public double GenA {
            get { return _GenA; }
            set { _GenA = value;
                OnPropertyChanged();
            }
        }

        private double _GenB;

        public double GenB {
            get { return _GenB; }
            set { _GenB = value;
            	OnPropertyChanged();
            }
        }

        private double _GenC;

        public double GenC {
            get { return _GenC; }
            set { _GenC = value;
                OnPropertyChanged();
            }
        }
		#endregion
		
		#region dla postaci prostej y = ax + b
        private double _SlopeA;

        public double SlopeA {
            get { return _SlopeA; }
            set { _SlopeA = value;
                OnPropertyChanged();
            }
        }

        private double _InterceptB;

        public double InterceptB {
            get { return _InterceptB; }
            set { _InterceptB = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private TS_point _Point1;

        public TS_point Point1 {
            get { return _Point1; }
            set { 
            	if (_Point1 != null)
            		_Point1.ParametersChanged -= ContainedElementParametersChanged;
            	
            	_Point1 = value;
            	
            	if (_Point1 != null) {
					_Point1.ParametersChanged += ContainedElementParametersChanged;
            	}
            	
                OnPropertyChanged();
				OnParametersChanged();
            }
        }
        private TS_point _Point2;

        public TS_point Point2 {
            get { return _Point2; }
            set { 
            	if (_Point2 != null)
            		_Point2.ParametersChanged -= ContainedElementParametersChanged;
            	
            	_Point2 = value;
            	
            	if (_Point2 != null) {
					_Point2.ParametersChanged += ContainedElementParametersChanged;
            	}
            	
                OnPropertyChanged();
				OnParametersChanged();
            }
        }
		
		public TS_line(TS_side division)
		{
			Point1 = division.StartPoint;
			Point2 = division.EndPoint;
			ReCalcMe();
		}
		
		public TS_line(TS_point pointA, TS_point pointB)
		{
			Point1 = pointA;
			Point2 = pointB;
			ReCalcMe();
		}
		
		public TS_line(double SlopeA, double InterceptB){
			this.SlopeA = SlopeA;
			this.InterceptB = InterceptB;
			GenB = -1;
			GenA = SlopeA;
			GenC = InterceptB;
			SetSomeMatchedPoints();
		}
		
		public TS_line(double A, double B, double C){
			this.GenA = A;
			this.GenB = B;
			this.GenC = C;
			SlopeA = GenA / (-GenB);
			InterceptB = GenC / (-GenB);
			SetSomeMatchedPoints();
		}
		
		#region Calculation and Checks
		private double SlopeFactorA()
		{
			if (TS_point.TS_AreDoublesEqual(Point1.X, Point2.X)) return Double.NaN;
			return (Point2.Y - Point1.Y)/(Point2.X - Point1.X);
		}
		
		private double InterceptFactorB()
		{
			double mySlopeA = SlopeFactorA();
			if (Double.IsNaN(mySlopeA)) return Double.NaN;
			return Point2.Y - (mySlopeA * Point2.X);
		}
		
		public void ReCalcMe()
		{
            if (Point1 != null && Point2 != null) {

				if (TS_point.TS_AreDoublesEqual(Point1.Y, Point2.Y) && TS_point.TS_AreDoublesEqual(Point1.X, Point2.X)) {
						SlopeA = Double.NaN;
                    	InterceptB = Double.NaN;
                    	GenA = Double.NaN;
                    	GenB = Double.NaN;
                    	GenC = Double.NaN;
					} else
                if (TS_point.TS_AreDoublesEqual(Point1.X, Point2.X)) {
					SlopeA = Double.PositiveInfinity;
                    InterceptB = Double.NaN;
                    GenA = 1;
                    GenB = 0;
                    GenC = -Point1.X;
                } else {
                    SlopeA = (Point2.Y - Point1.Y) / (Point2.X - Point1.X);
                    InterceptB = Point2.Y - (SlopeA * Point2.X);
                    GenA = SlopeA;
                    GenB = -1;
                    GenC = InterceptB;
                }
            }
		}
		
		private double GetValueOf_Yaxis(double x){
			return (-GenA * x - GenC) / GenB;
		}
		
		private double GetValueOf_Xaxis(double y){
			return (-GenB * y - GenC) / GenA;
		}
		
		public TS_point Intersection(TS_line line2)
		{
			double a1 = SlopeA;
			double b1 = InterceptB;
			double a2 = line2.SlopeA;
			double b2 = line2.InterceptB;
			
			if (TS_point.TS_AreDoublesEqual(a1, a2)) return new TS_point(Double.NaN, Double.NaN);
			/*
			double x = (b1-b2)/(a2-a1);
			double y = (a2*b1 - b2*a1)/(a2-a1);
			return new TS_point(x,y);
			*/
			
			double A1 = GenA;
			double A2 = line2.GenA;
			double B1 = GenB;
			double B2 = line2.GenB;
			double C1= GenC;
			double C2 = line2.GenC;
			
			double D1 = (B2-B1)/(A1-A2);
			double D2 = (C2-C1)/(A1-A2);
			
			double y = -(A1 * D2 + C1) / (A1 * D1 + B1);
			double x = D1 * y + D2;
			
			return new TS_point(x,y);
		}

        public bool IsCrossedWith(TS_line line2)
        {
            TS_point crossPoint = Intersection(line2);
            if (Double.IsNaN(crossPoint.X) || Double.IsNaN(crossPoint.Y))
            {
                return false;
            }
            return true;
        }

        public bool IsContain(TS_point point)
        {
            if (TS_point.TS_AreDoublesEqual(GenA * point.X + GenB * point.Y + GenC, 0))
            {
                return true;
            }
            return false;
        }
        
        private void SetSomeMatchedPoints(){
        	if (TS_point.TS_AreDoublesEqual(GenB,0) ) {
				_Point1 = new TS_point(_GenC / _GenA, 0);
				_Point2 = new TS_point(_GenC / _GenA, 1);
        	}
        	else{
        		_Point1 = new TS_point(0, GetValueOf_Yaxis(0));
				_Point2 = new TS_point(1, GetValueOf_Yaxis(1));
        	}
        }
		
        #endregion
        
		public override string ToString()
		{
			string format = "{0:0.###}";
			string A = String.Format(format, GenA);
			string B = String.Format(format, GenB);
			string C = String.Format(format, GenC);
			string a = String.Format(format, SlopeA);
			string b = String.Format(format, InterceptB);
			
			return A + "x+" + B + "y+" + C + "=0";
        }

        #region InotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName="") {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region InotifyParametersChanged Members
        public event EventHandler ParametersChanged;

        private void OnParametersChanged() {
            EventHandler handler = ParametersChanged;

            if (handler != null) {
                handler(this, new EventArgs());
            }
        }
        #endregion
        
		void ContainedElementParametersChanged(object sender, EventArgs args)
		{
			ReCalcMe();
			//OnParametersChanged();
		}
    }
}
