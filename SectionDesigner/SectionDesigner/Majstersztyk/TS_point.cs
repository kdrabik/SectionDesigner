/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 05/12/2018
 * Time: 12:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Majstersztyk
{
	/// <summary>
	/// Description of TS_point.
	/// </summary>
	public class TS_point:INotifyPropertyChanged
	{
        private double _X;

        public double X {
            get { return _X; }
            set { _X = value;
                OnPropertyChanged();
            }
        }

        private double _Y;

        public double Y {
            get { return _Y; }
            set { _Y = value;
                OnPropertyChanged();
            }
        }
		
		public TS_point(){
			_X = Double.NaN;
			_Y = Double.NaN;
		}
		
		public TS_point(double X, double Y)
		{
			_X = X;
			_Y = Y;
		}

        public bool IsReal(){
			if (Double.IsNaN(X) || Double.IsNaN(Y))
				return false;
			return true;
		}
		
		public TS_point Transform(TS_point punkt0){
			return new TS_point(X-punkt0.X, Y-punkt0.Y);
		}
		
		public static bool TS_AreDoublesEqual(double d1, double d2)
		{
			if (Double.IsInfinity(d1) && !Double.IsInfinity(d2)) {
				return false;
			}
			
			if (!Double.IsInfinity(d1) && Double.IsInfinity(d2)) {
				return false;
			}
			
			double diff = Math.Abs(Math.Min(d1, d2) / 1000000);
			
			if (Math.Abs(d1-d2) <= diff) return true;
			
			return false;
		}
		
		public override string ToString()
		{
			return "[" + string.Format("{0:0.000}",X) + " ; " + string.Format("{0:0.000}",Y) + "]";
		}
		
		public bool IsTheSame(TS_point point2){
			if (TS_AreDoublesEqual(X, point2.X) && TS_AreDoublesEqual(Y, point2.Y)) return true;
			return false;
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
        
        #region Equals and GetHashCode implementation 
  /*      public override bool Equals(object obj)
		{
			TS_point other = obj as TS_point;
				if (other == null)
					return false;
			return TS_AreDoublesEqual(this.X, other.X) && TS_AreDoublesEqual(this.Y, other.Y);
		}
        */
        /*
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * X.GetHashCode();
				hashCode += 1000000009 * Y.GetHashCode();
			}
			return hashCode;
		}
		public static bool operator ==(TS_point lhs, TS_point rhs) {
			if (TS_AreDoublesEqual(lhs.X, rhs.X) && TS_AreDoublesEqual(lhs.Y, rhs.Y))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(TS_point lhs, TS_point rhs) {
			return !(lhs == rhs);
		}
        */
        #endregion

    }
}
