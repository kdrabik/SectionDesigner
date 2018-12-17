/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 05/12/2018
 * Time: 12:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Majstersztyk
{
	/// <summary>
	/// Description of TS_point.
	/// </summary>
	public class TS_point
	{
		public double X {get; private set;}
		public double Y {get; private set;}
		
		public TS_point(double X, double Y)
		{
			this.X = X;
			this.Y = Y;
		}
		
		public TS_point Transform(TS_point punkt0){
			return new TS_point(X-punkt0.X, Y-punkt0.Y);
		}
		
		public static bool TS_AreDoublesEqual(double d1, double d2)
		{
			double diff = Math.Min(d1,d2)/1000000;
			
			if (Math.Abs(d1-d2) <= diff) return true;
			
			return false;
		}
		
		public override string ToString()
		{
			return "[" + string.Format("{0:0.000}",X) + " ; " + string.Format("{0:0.000}",Y) + "]";
		}

	}
}
