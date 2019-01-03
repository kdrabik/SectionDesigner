/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 05/12/2018
 * Time: 12:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Majstersztyk
{
	/// <summary>
	/// Description of TS_side.
	/// </summary>
	public class TS_side:INotifyPropertyChanged
	{
		public TS_line Line {get; private set;}

        private TS_point _StartPoint;

        public TS_point StartPoint {
            get { return _StartPoint; }
            set {
                if (_StartPoint != null)
                    _StartPoint.PropertyChanged -= Side_PropertyChanged;

                _StartPoint = value;

                if (_StartPoint != null) {
                    _StartPoint.PropertyChanged += Side_PropertyChanged;
                }

                OnPropertyChanged();
            }
        }

        private TS_point _EndPoint;

        public TS_point EndPoint {
            get { return _EndPoint; }
            set {
                if (_EndPoint != null)
                    _EndPoint.PropertyChanged -= Side_PropertyChanged;

                _EndPoint = value;

                if (_EndPoint != null) {
                    _EndPoint.PropertyChanged += Side_PropertyChanged;
                }

                OnPropertyChanged();
            }
        }
        		
		public TS_side(TS_point StartPoint, TS_point EndPoint)
		{
			this.StartPoint = StartPoint;
			this.EndPoint = EndPoint;
            Line = new TS_line(StartPoint, EndPoint);
            ReCalcMe();
		}		
		
        private void ReCalcMe() {
            Line.ReCalcMe();
        }

		public double Length()
		{
			return Math.Sqrt(Math.Pow((EndPoint.X-StartPoint.X),2) + Math.Pow((EndPoint.Y-StartPoint.Y),2));
		}
		
		public bool IsCrossedWith(TS_side side2){
            TS_point crossPoint = Line.Intersection(side2.Line);
            if (IsContain(crossPoint) && side2.IsContain(crossPoint))
            {
                return true;
            }
            return false;
		}
		
        public bool IsCrossedWith(TS_line line2)
        {
            if (Line.IsCrossedWith(line2))
            {
                TS_point crossPoint = Line.Intersection(line2);
                if (IsContain(crossPoint))
                {
                    return true;
                }
            }
            return false;
        }
        
        public TS_point CrossedPoint(TS_side side2){
			TS_point point = Line.Intersection(side2.Line);
			
			if (IsContain(point))
				return point;
			
			return new TS_point();
        }
        
        public TS_point CrossedPoint(TS_line line2){
			TS_point point = Line.Intersection(line2);
			
			if (IsContain(point))
				return point;
			
			return new TS_point();
        }

        public bool IsContain(TS_point point)
        {
            double x = point.X;
            double y = point.Y;
            if (point.IsReal() && Line.IsContain(point))
            {
                double minX1 = Math.Min(StartPoint.X, EndPoint.X);
                double maxX1 = Math.Max(StartPoint.X, EndPoint.X);
                double minY1 = Math.Min(StartPoint.Y, EndPoint.Y);
                double maxY1 = Math.Max(StartPoint.Y, EndPoint.Y);

                if (x >= minX1 && x <= maxX1 && y >= minY1 && y <= maxY1)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        
		public override string ToString()
		{
			return StartPoint.ToString() + " \t-->\t" + EndPoint.ToString();
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

        void Side_PropertyChanged(object sender, PropertyChangedEventArgs args) {
            ReCalcMe();
        }
    }
}
