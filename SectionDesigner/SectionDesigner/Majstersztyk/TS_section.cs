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
using System.ComponentModel;
using System.Linq;
using SectionDesigner;

namespace Majstersztyk
{
	/// <summary>
	/// Description of TS_section.
	/// </summary>
	public class TS_section : TS_region
	{
        private ObservableCollectionEx<TS_part> _Parts;

        public ObservableCollectionEx<TS_part> Parts {
            get { return _Parts; }
            set {
                if (_Parts != null)
                    _Parts.ParametersChanged -= Section_OnParametersChanged;

                _Parts = value;

                if (_Parts != null) {
                    _Parts.ParametersChanged += Section_OnParametersChanged;
                }
                OnPropertyChanged();
				OnParametersChanged();
            }
        }

        private ObservableCollectionEx<TS_reinforcement> _Reinforcement;

        public ObservableCollectionEx<TS_reinforcement> Reinforcement {
            get { return _Reinforcement; }
            set {
                if (_Reinforcement != null)
                    _Reinforcement.ParametersChanged -= Section_OnParametersChanged;

                _Reinforcement = value;

                if (_Reinforcement != null) {
                    _Reinforcement.ParametersChanged += Section_OnParametersChanged;
                }
                OnPropertyChanged();
				OnParametersChanged();
            }
        }
        
        protected TS_region _SelectedPartOrReo;
        public TS_region SelectedPartOrReo{
        	get{ return _SelectedPartOrReo;}
        	set{
                _SelectedPartOrReo = value;
        		OnSelectedMemberChanged();} }
		
        
        public override string TypeOf { get { return typeOf; } }
        private new readonly string typeOf = "Section";

        public TS_section() {
            Parts = new ObservableCollectionEx<TS_part>();
            Reinforcement = new ObservableCollectionEx<TS_reinforcement>();
            Name = "Section_1";
        }
				
		public TS_section(List<TS_part> parts, List<TS_reinforcement> reinforcement):this()
		{
            Parts.AddRange(parts);
            Reinforcement.AddRange(reinforcement);
			CalcProperties();
		}
        
        #region Calculation
        protected override double CalcArea()
        {
            double area = 0;
            double E0 = _Parts[0].Material.E;

            foreach (TS_part Part in _Parts)
            {
                area += Part.Area * Part.Material.E / E0;
            }

            foreach (TS_reinforcement Reo_group in _Reinforcement) {
                area += Reo_group.Area * Reo_group.Material.E / E0;
            }
            return area;
        }

        protected override double CalcSx()
        {
            double sx = 0;
            double E0 = _Parts[0].Material.E;

            foreach (TS_part Part in _Parts)
            {
                sx += Part.StaticMomX * Part.Material.E / E0;
            }

            foreach (TS_reinforcement Reo_group in _Reinforcement) {
                sx += Reo_group.StaticMomX * Reo_group.Material.E / E0;
            }
            return sx;
        }

        protected override double CalcSy()
        {
            double sy = 0;
            double E0 = _Parts[0].Material.E;

            foreach (TS_part Part in _Parts)
            {
                sy += Part.StaticMomY * Part.Material.E / E0;
            }

            foreach (TS_reinforcement Reo_group in _Reinforcement) {
                sy += Reo_group.StaticMomY * Reo_group.Material.E / E0;
            }
            return sy;
        }

        protected override double CalcIx()
        {
            double ix = 0;
            double E0 = _Parts[0].Material.E;

            foreach (TS_part Part in _Parts)
            {
                ix += Part.InertiaMomX * Part.Material.E / E0;
            }

            foreach (TS_reinforcement Reo_group in _Reinforcement) {
                ix += Reo_group.InertiaMomX * Reo_group.Material.E / E0;
            }
            return ix;
        }

        protected override double CalcIy()
        {
            double iy = 0;
            double E0 = _Parts[0].Material.E;

            foreach (TS_part Part in _Parts)
            {
                iy += Part.InertiaMomX * Part.Material.E / E0;
            }

            foreach (TS_reinforcement Reo_group in _Reinforcement) {
                iy += Reo_group.InertiaMomY * Reo_group.Material.E / E0;
            }
            return iy;
        }

        protected override double CalcIxy()
        {
            double ixy = 0;
            double E0 = _Parts[0].Material.E;

            foreach (TS_part Part in _Parts)
            {
                ixy += Part.DeviationMomXY * Part.Material.E / _Parts[0].Material.E;
            }

            foreach (TS_reinforcement Reo_group in _Reinforcement) {
                ixy += Reo_group.DeviationMomXY * Reo_group.Material.E / E0;
            }
            return ixy;
        }

        protected override void CalcCentralProp() {
            double centrx = 0;
            double centry = 0;
            double centrxy = 0;

            foreach (var Part in _Parts) {
                centrx += (Part.InertiaMomX - Math.Pow(Centroid.Y, 2) * Part.Area) 
                    * Part.Material.E / _Parts[0].Material.E;
                centry += (Part.InertiaMomY - Math.Pow(Centroid.X, 2) * Part.Area) 
                    * Part.Material.E / _Parts[0].Material.E;
                centrxy += (Part.DeviationMomXY - (Centroid.X * Centroid.Y) * Part.Area) 
                    * Part.Material.E / _Parts[0].Material.E;
            }

            foreach (var Reo in _Reinforcement) {
                centrx += (Reo.InertiaMomX - Math.Pow(Centroid.Y, 2) * Reo.Area)
                    * Reo.Material.E / _Parts[0].Material.E;
                centry += (Reo.InertiaMomY - Math.Pow(Centroid.X, 2) * Reo.Area)
                    * Reo.Material.E / _Parts[0].Material.E;
                centrxy += (Reo.DeviationMomXY - (Centroid.Y * Centroid.X) * Reo.Area)
                    * Reo.Material.E / _Parts[0].Material.E;
            }

            CentrInertiaMom_X = centrx;
            CentrInertiaMom_Y = centry;
            CentrDeviationMom_XY = centrxy;
        }

        protected override bool IsObjectCorrect() {

            foreach (TS_reinforcement Reo_group in _Reinforcement) {
                if (!Reo_group.IsCorrect) return false;
            }

            foreach (var Part in _Parts) {
                if (!Part.IsCorrect) return false;
            }

            for (int i = 0; i < _Parts.Count; i++) {
                for (int j = 0; j < _Parts.Count; j++) {
                    if (i != j) {
                        foreach (var node in _Parts[i].Contour.Vertices) {
                            if (_Parts[j].Contour.IsPointInside(node)) return false;
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

        #endregion

        #region Helps

        public override string ToString()
		{
			string text = "";
			text += base.ToString();
			
			foreach (var tenPart in _Parts) {
				text += tenPart.ToString();
			}
			
			foreach (var reoGroup in _Reinforcement) {
				text += reoGroup.ToString();
			}
			
			return text;
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
        #endregion

        protected void Section_OnParametersChanged(object sender, EventArgs args) {
            if (_Parts != null && _Reinforcement != null) {
                CalcProperties();
				OnParametersChanged();
                OnPropertyChanged("IsCorrect");
            }
        }
    }
}
