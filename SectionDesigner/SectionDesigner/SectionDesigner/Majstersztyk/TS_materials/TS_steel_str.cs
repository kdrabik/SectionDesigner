/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 12/11/2018
 * Time: 16:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Majstersztyk.TS_materials
{
	/// <summary>
	/// Description of TS_steel_str.
	/// </summary>
	public class TS_steel_str:TS_material
	{		
		public double fy;
		
		
		public static List<string> Yield = new List<string>{
			"215 MPa", "235 MPa", "255 MPa", "275 MPa", "335 MPa", "355 MPa", "410 MPa", "440 MPa"
		};
		
		public TS_steel_str(string Yield)
		{
			switch (Yield) {
				case "215 MPa":
					fy = 215;
					break;
				case "235 MPa":
					fy = 235;
					break;
				case "255 MPa":
					fy = 255;
					break;
				case "275 MPa":
					fy = 275;
					break;
				case "335 MPa":
					fy = 335;
					break;
				case "355 MPa":
					fy = 355;
					break;
				case "410 MPa":
					fy = 410;
					break;
				case "440 MPa":
					fy = 440;
					break;
			}
			E = 210000;
		}
		
		public TS_steel_str(double Yield, double ElastiModulus){
			this.fy = Yield;
			this.E = ElastiModulus;
		}
		
		public double SigmaS(double epsilon)
        {
        	return epsilon * E / 1000;
        }
	}
}
