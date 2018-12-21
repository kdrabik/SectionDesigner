/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 12/11/2018
 * Time: 10:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Majstersztyk.TS_materials
{
	/// <summary>
	/// Description of TS_steel.
	/// </summary>
	public class TS_steel_rf: TS_material, IComparable<TS_steel_rf>
	{
		public static List<string> Yield = new List<string>{
			"400 MPa", "500 MPa", "600 MPa"
		};
		
		public static List<string> Class = new List<string>{
			"Class A", "Class B", "Class C" 
		};
		
		private bool Inclined;
		
		public double fyk { get; private set; }
        public double epsilon_uk { get; private set; }
        public double k {get; private set; }
		
		public TS_steel_rf(string Class, string Yield, bool Inclined)
		{
			if (Inclined) {
				switch (Class) {
					case "Class A":
						k = 1.05;
						epsilon_uk = 2.5;
						break;
					case "Class B":
						k = 1.08;
						epsilon_uk = 5;
						break;
					case "Class C":
						k = 1.15;
						epsilon_uk = 7.5;
						break;
				}
			} else {
				k = 1;
				epsilon_uk = 2.5;
			}
			
			switch (Yield) {
				case "400 MPa":
					fyk = 400;
					break;
				case "500 MPa":
					fyk = 500;
					break;
				case "600 MPa":
					fyk = 600;
					break;
			}
			E = 200000;
		}
		
		public TS_steel_rf(double ElasticityModule, double Yield, double k, double epsilon_uk, bool Inclined){
			fyk = Yield;
			this.k = k;
			this.epsilon_uk = epsilon_uk;
			this.Inclined = Inclined;
			this.E = ElasticityModule;
		}
		
		public int CompareTo(TS_steel_rf other)
        {
            if (this.fyk == other.fyk)
            {
                return 0;
            }
            else if (this.fyk < other.fyk)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public override bool Equals(object obj)
        {
            TS_steel_rf other = obj as TS_steel_rf;
            if (this.fyk == other.fyk && this.epsilon_uk == other.epsilon_uk && this.k == other.k)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public double SigmaS(double epsilon, double gammaS)
        {
        	double firstpointX = fyk / E / gammaS;
			double firstpointY = fyk / gammaS;
			double endPointY = k * fyk / gammaS;
			
            if (Math.Abs(epsilon) / 1000 < firstpointX) {
                return epsilon * E / 1000;
            }
            if (Inclined) {
				return fyk / gammaS + epsilon * (endPointY - firstpointY) / (epsilon_uk - firstpointX);
            } 
			return Math.Abs(epsilon) / epsilon * fyk / gammaS;
        }
        
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
