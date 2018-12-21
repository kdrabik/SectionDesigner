﻿/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 06/12/2018
 * Time: 11:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Majstersztyk
{
	/// <summary>
	/// Description of TS_section.
	/// </summary>
	public class TS_section:TS_region
	{
		public List<TS_part> Parts { get; private set; }
        public List<TS_reinforcement> ReinforcementEntire { get; private set; }
		public List<TS_reinforcement> ReinforcementFree { get; private set; }

        public override string TypeOf { get { return typeOf; } }
        private string typeOf = "Section";
        /*
		public double Area {get; private set;}
		public double StaticMomX {get; private set;}
		public double StaticMomY {get; private set;}
		public double InertiaMomX {get; private set;}
		public double InertiaMomY {get; private set;}
		public double DeviationMomXY {get; private set;}
		public TS_point Centroid {get; private set;}
        */
        public TS_section() {
            Parts = new List<TS_part>();
            ReinforcementEntire = new List<TS_reinforcement>();
        }
				
		public TS_section(List<TS_part> parts, List<TS_reinforcement> reinforcement)
		{
            Update(parts, reinforcement);
		}

        public void Update(List<TS_part> parts, List<TS_reinforcement> reinforcement)
        {
            Parts = parts;
            ReinforcementEntire = reinforcement;
			SeperateReinforcementForParts();
            CalcProperties();
        }
        
        protected override double CalcArea()
        {
            double area = 0;
            double E0 = Parts[0].Material.E;

            foreach (TS_part Part in Parts)
            {
                area += Part.Area * Part.Material.E / E0;
                foreach (var Reo in Part.BelongingReinforcement) {
					area += Reo.Area * (Reo.Material.E - Part.Material.E) / E0;
                }
            }

            foreach (TS_reinforcement Reo_group in ReinforcementFree) {
                area += Reo_group.Area * Reo_group.Material.E / E0;
            }
            return area;
        }

        protected override double CalcSx()
        {
            double sx = 0;
            double E0 = Parts[0].Material.E;

            foreach (TS_part Part in Parts)
            {
                sx += Part.StaticMomX * Part.Material.E / E0;
                foreach (var Reo in Part.BelongingReinforcement) {
					sx += Reo.StaticMomX * (Reo.Material.E - Part.Material.E) / E0;
                }
            }

            foreach (TS_reinforcement Reo_group in ReinforcementFree) {
                sx += Reo_group.StaticMomX * Reo_group.Material.E / E0;
            }
            return sx;
        }

        protected override double CalcSy()
        {
            double sy = 0;
            double E0 = Parts[0].Material.E;

            foreach (TS_part Part in Parts)
            {
                sy += Part.StaticMomY * Part.Material.E / E0;
                foreach (var Reo in Part.BelongingReinforcement) {
					sy += Reo.StaticMomY * (Reo.Material.E - Part.Material.E) / E0;
                }
            }

            foreach (TS_reinforcement Reo_group in ReinforcementFree) {
                sy += Reo_group.StaticMomY * Reo_group.Material.E / E0;
            }
            return sy;
        }

        protected override double CalcIx()
        {
            double ix = 0;
            double E0 = Parts[0].Material.E;

            foreach (TS_part Part in Parts)
            {
                ix += Part.InertiaMomX * Part.Material.E / E0;
                foreach (var Reo in Part.BelongingReinforcement) {
					ix += Reo.InertiaMomX * (Reo.Material.E - Part.Material.E) / E0;
                }
            }

            foreach (TS_reinforcement Reo_group in ReinforcementFree) {
                ix += Reo_group.InertiaMomX * Reo_group.Material.E / E0;
            }
            return ix;
        }

        protected override double CalcIy()
        {
            double iy = 0;
            double E0 = Parts[0].Material.E;

            foreach (TS_part Part in Parts)
            {
                iy += Part.InertiaMomX * Part.Material.E / E0;
                foreach (var Reo in Part.BelongingReinforcement) {
					iy += Reo.InertiaMomY * (Reo.Material.E - Part.Material.E) / E0;
                }
            }

            foreach (TS_reinforcement Reo_group in ReinforcementFree) {
                iy += Reo_group.InertiaMomY * Reo_group.Material.E / E0;
            }
            return iy;
        }

        protected override double CalcIxy()
        {
            double ixy = 0;
            double E0 = Parts[0].Material.E;

            foreach (TS_part Part in Parts)
            {
                ixy += Part.DeviationMomXY * Part.Material.E / Parts[0].Material.E;
                foreach (var Reo in Part.BelongingReinforcement) {
					ixy += Reo.DeviationMomXY * (Reo.Material.E - Part.Material.E) / E0;
                }
            }

            foreach (TS_reinforcement Reo_group in ReinforcementFree) {
                ixy += Reo_group.DeviationMomXY * Reo_group.Material.E / E0;
            }
            return ixy;
        }

        protected override void CalcCentralProp() {
            double centrx = 0;
            double centry = 0;
            double centrxy = 0;
            double E0 = Parts[0].Material.E;

            foreach (var Part in Parts) {
                centrx += (Part.InertiaMomX - Math.Pow(Centroid.Y, 2) * Part.Area) 
                    * Part.Material.E / E0;
                centry += (Part.InertiaMomY - Math.Pow(Centroid.X, 2) * Part.Area) 
                    * Part.Material.E / E0;
                centrxy += (Part.DeviationMomXY - (Centroid.X * Centroid.Y) * Part.Area) 
                    * Part.Material.E / E0;
                
                foreach (var Reo in Part.BelongingReinforcement) {
                	centrx += (Reo.InertiaMomX - Math.Pow(Centroid.Y, 2) * Reo.Area) * (Reo.Material.E - Part.Material.E) / E0;
					centry += (Reo.InertiaMomY - Math.Pow(Centroid.X, 2) * Reo.Area) * (Reo.Material.E - Part.Material.E) / E0;
					centrxy += (Reo.DeviationMomXY - (Centroid.Y * Centroid.X) * Reo.Area) * (Reo.Material.E - Part.Material.E) / E0;
                }
            }

            foreach (var Reo in ReinforcementFree) {
                centrx += (Reo.InertiaMomX - Math.Pow(Centroid.Y, 2) * Reo.Area) * Reo.Material.E / Parts[0].Material.E;
                centry += (Reo.InertiaMomY - Math.Pow(Centroid.X, 2) * Reo.Area) * Reo.Material.E / Parts[0].Material.E;
                centrxy += (Reo.DeviationMomXY - (Centroid.Y * Centroid.X) * Reo.Area) * Reo.Material.E / Parts[0].Material.E;
            }

            CentrInertiaMom_X = centrx;
            CentrInertiaMom_Y = centry;
            CentrDeviationMom_XY = centrxy;
        }

        protected override bool IsObjectCorrect() {

            for (int i = 0; i < ReinforcementEntire.Count; i++) {
        		if (!ReinforcementEntire[i].IsCorrect) return false;
        		for (int j = 0; j < ReinforcementEntire.Count; j++) {
        			if (i!=j) {
        				foreach (var bar1 in ReinforcementEntire[i].Bars) {
        					foreach (var bar2 in ReinforcementEntire[j].Bars) {
        						double mindist = (bar1.Diameter + bar2.Diameter)/2;
        						double dist = Math.Sqrt(Math.Pow(bar1.coordinates.X-bar2.coordinates.X,2)+Math.Pow(bar1.coordinates.Y-bar2.coordinates.Y,2));
								if (dist < mindist) return false;
        					}
        				}
        			}
        		}
            }
        	
            foreach (var Part in Parts) {
                if (!Part.IsCorrect) return false;
            }

            for (int i = 0; i < Parts.Count; i++) {
                for (int j = 0; j < Parts.Count; j++) {
                    if (i != j) {
                        foreach (var node in Parts[i].Contour.Vertices) {
                            if (Parts[j].Contour.IsPointInside(node)) return false;
                        }
                    }
                }
            }
            
            return true;
        }

        private double CalcPrincipleAngle() {
            double tg2fi0;
            if (TS_section.AreDoublesEqual(CentrInertiaMom_X, CentrInertiaMom_Y)) {
                return 0;
            }
            tg2fi0 = 2 * CentrDeviationMom_XY / (CentrInertiaMom_X - CentrInertiaMom_Y);
            return (Math.Atan(tg2fi0) / 2);// - Math.PI / 2;
        }
        
        public override string ToString()
		{
			string text = "";
			text += base.ToString();
			
			foreach (var tenPart in Parts) {
				text += tenPart.ToString();
			}
			
			text += Environment.NewLine + Environment.NewLine + "ENTIRE REINFORCEMENT IN THE SECTION:";
			
			foreach (var reoGroup in ReinforcementEntire) {
				text += Environment.NewLine + reoGroup.ToString();
			}
			
			return text;
		}
        
        private void SeperateReinforcementForParts(){
			ReinforcementFree = ReinforcementEntire;
			
        	for (int i = 0; i < ReinforcementEntire.Count; i++) {
				foreach (var part in Parts) {
					ReinforcementFree[i] = part.TakeBelongingReinforcement(ReinforcementFree[i]);
				}
			}
				/*
        		foreach (var bar in reoGroup.Bars) {
					bool IsBarFree = false;
        			foreach (var part in Parts) {
						if (part.AddBelongingReinforcement(bar)) break;
						IsBarFree = true;
        			}
					if (IsBarFree) BarsGroup.Add(bar);
        		}
				ReinforcementFree.Add(new TS_reinforcement(BarsGroup, reoGroup.Material));*/
        	}

        

        /*
		public static List<TS_point> TransformByMoving(List<TS_point> Vertices, TS_point newCenterPoint) {
			List<TS_point> newVertices = new List<TS_point>();
			for (int i = 0; i < Vertices.Count; i++) {
				newVertices.Add(new TS_point(Vertices[i].X - newCenterPoint.X, Vertices[i].Y - newCenterPoint.Y));
			}
			return newVertices;
		}
		
		public static List<TS_point> TransformByRotating(List<TS_point> Vertices, double angle){
			List<TS_point> newVertices = new List<TS_point>();
			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);
			for (int i = 0; i < Vertices.Count; i++) {
				double x = Vertices[i].X * cos - Vertices[i].Y * sin;
				double y = Vertices[i].X * sin + Vertices[i].Y * cos;
				newVertices.Add(new TS_point(x, y));
			}
			return newVertices;
		}
		
		public static List<TS_point> TransformByMovingAndRotating(List<TS_point> Vertices, TS_point newCenterPoint, double angle){
			return TransformByRotating(TransformByMoving(Vertices, newCenterPoint), angle);
		}
		*/

    }
}
