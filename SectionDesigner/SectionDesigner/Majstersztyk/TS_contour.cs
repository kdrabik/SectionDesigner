/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 12/11/2018
 * Time: 16:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SectionDesigner;

namespace Majstersztyk
{
	/// <summary>
	/// Description of TS_contour.
	/// </summary>
	public class TS_contour : TS_region, INotifyPropertyChanged
    {
        private ObservableList<TS_point> _Vertices;

        public ObservableList<TS_point> Vertices {
            get { return _Vertices; }
            set {
                if (_Vertices != null) {
                    _Vertices.PropertyChanged -= Contour_OnPropertyChanged;
                    _Vertices.CollectionChanged -= Contour_OnCollectionChanged;
                }

                _Vertices = value;

                if (_Vertices != null) {
                    _Vertices.PropertyChanged += Contour_OnPropertyChanged;
                    _Vertices.CollectionChanged += Contour_OnCollectionChanged;
                }

                OnPropertyChanged();
            }
        }

        public List<TS_side> Sides { get { return GenerateSides(Vertices); } }
        public override string TypeOf { get { return typeOf; } }
        private new readonly string typeOf = "Contour";
        
        protected TS_contour() {
            Vertices = new ObservableList<TS_point>();
            Name = "Contour";
            CalcProperties();
        }

        public TS_contour(List<TS_point> vertices) : this() {
            Vertices.AddRange(vertices);

            if (CalcArea() < 0) {
                Vertices.Reverse();
            }

            //Sides.AddRange(GenerateSides(Vertices));
        }

        #region Calculations
        protected override double CalcArea() {
            double A = 0;
            for (int i = 0; i < _Vertices.Count; i++)
            {
                int j = IndexOfNextVertex(i);
                A += ((_Vertices[j].X - _Vertices[i].X) * (_Vertices[j].Y + _Vertices[i].Y));
            }
            return 1d / 2 * A;
        }

        protected override double CalcSx() {
            double Sx = 0;
            for (int i = 0; i < _Vertices.Count; i++)
            {
                int j = IndexOfNextVertex(i);
                Sx += ((_Vertices[j].X - _Vertices[i].X) * (_Vertices[i].Y * _Vertices[i].Y + _Vertices[i].Y * _Vertices[j].Y + _Vertices[j].Y * _Vertices[j].Y));
            }
            return 1d / 6 * Sx;
        }

        protected override double CalcSy() {
            double Sy = 0;
            for (int i = 0; i < _Vertices.Count; i++)
            {
                int j = IndexOfNextVertex(i);
                Sy += ((_Vertices[i].Y - _Vertices[j].Y) * (_Vertices[i].X * _Vertices[i].X + _Vertices[i].X * _Vertices[j].X + _Vertices[j].X * _Vertices[j].X));
            }
            return 1d / 6 * Sy;
        }

        protected override double CalcIx() {
            double Ix = 0;
            for (int i = 0; i < _Vertices.Count; i++)
            {
                int j = IndexOfNextVertex(i);
                Ix += (_Vertices[j].X - _Vertices[i].X) *
                    (_Vertices[i].Y * _Vertices[i].Y * _Vertices[i].Y +
                     _Vertices[i].Y * _Vertices[i].Y * _Vertices[j].Y +
                     _Vertices[i].Y * _Vertices[j].Y * _Vertices[j].Y +
                     _Vertices[j].Y * _Vertices[j].Y * _Vertices[j].Y);
            }
            return 1d / 12 * Ix;
        }

        protected override double CalcIy() {
            double Iy = 0;
            for (int i = 0; i < _Vertices.Count; i++)
            {
                int j = IndexOfNextVertex(i);
                Iy += (_Vertices[i].Y - _Vertices[j].Y) *
                    (_Vertices[i].X * _Vertices[i].X * _Vertices[i].X +
                     _Vertices[i].X * _Vertices[i].X * _Vertices[j].X +
                     _Vertices[i].X * _Vertices[j].X * _Vertices[j].X +
                     _Vertices[j].X * _Vertices[j].X * _Vertices[j].X);
            }
            return 1d / 12 * Iy;
        }

        protected override double CalcIxy() {
            double Ixy = 0;
            for (int i = 0; i < _Vertices.Count; i++)
            {
                int j = IndexOfNextVertex(i);
                Ixy += (_Vertices[j].X - _Vertices[i].X) *
                    (_Vertices[i].X *
                     (3 * _Vertices[i].Y * _Vertices[i].Y +
                      _Vertices[j].Y * _Vertices[j].Y +
                      2 * _Vertices[i].Y * _Vertices[j].Y) +
                     (_Vertices[j].X *
                      (3 * _Vertices[j].Y * _Vertices[j].Y +
                       _Vertices[i].Y * _Vertices[i].Y +
                       2 * _Vertices[i].Y * _Vertices[j].Y)));
            }
            return 1d / 24 * Ixy;
        }
        #endregion

        #region Helps
        protected int IndexOfNextVertex(int currentVertex)
        {
            return (currentVertex + 1) % _Vertices.Count;
        }

        protected static List<TS_side> GenerateSides(ObservableList<TS_point> points)
        {
            List<TS_side> sides = new List<TS_side>();
            for (int i = 0; i < points.Count; i++)
            {
                int j = (i + 1) % points.Count;
                sides.Add(new TS_side(points[i], points[j]));
            }
            return sides;
        }

        protected override bool IsObjectCorrect()
        {
            if (_Vertices.Count < 3)
                return false;
            
            for (int i = 0; i < Sides.Count; i++)
            {
                for (int j = 0; j < Sides.Count; j++)
                {
                    if (Math.Abs(i - j) > 1)
                        if ((Math.Abs(i - j) + 1) != Sides.Count)
                            if (Sides[i].IsCrossedWith(Sides[j]))
                                return false;
                }
            }
            return true;
        }
        
        public bool IsPointInside(TS_point point){
			if (IsAVertex(point)) return false;
			
			foreach (var side in Sides) {
				if (side.IsContain(point)) return false;
			}
			
			TS_line horLine = new TS_line(0, point.Y);
			ObservableList<TS_point> horPoints = new ObservableList<TS_point>();
			
			foreach (var side in Sides) {
				TS_point pointX = side.CrossedPoint(horLine);
				if (pointX.X > point.X && !horPoints.Contains(pointX)) {
					horPoints.Add(pointX);
					}
				}
			
			int countH = horPoints.Count;
			if (countH % 2 != 0) return true;
			
			TS_line vertLine = new TS_line(1, 0, -point.X);
			List<TS_point> vertPoints = new List<TS_point>();
			
			foreach (var side in Sides) {
				TS_point pointY = side.CrossedPoint(vertLine);
				if (pointY.Y > point.Y && !vertPoints.Contains(pointY)) {
					vertPoints.Add(pointY);
				}
			}
			
			int countV = vertPoints.Count;
			if (countV % 2 != 0) return true;
			
			TS_line horLineL = new TS_line(0, point.Y);
			List<TS_point> horPointsL = new List<TS_point>();
			
			foreach (var side in Sides) {
				TS_point pointX = side.CrossedPoint(horLineL);
				if (pointX.X > point.X && !horPointsL.Contains(pointX)) {
					horPointsL.Add(pointX);
					}
				}
			
			int countHL = horPointsL.Count;
			if (countHL % 2 != 0) return true;
			
			List<TS_point> vertPointsD = new List<TS_point>();
			
			foreach (var side in Sides) {
				TS_point pointY = side.CrossedPoint(vertLine);
				if (pointY.Y > point.Y && !vertPointsD.Contains(pointY)) {
					vertPointsD.Add(pointY);
				}
			}
			
			int countVD = vertPointsD.Count;
			if (countVD % 2 != 0) return true;
			
			return false;
        }
        
        public bool IsAVertex(TS_point point){
        	foreach (var vertex in _Vertices) {
        		if (AreDoublesEqual(vertex.X,point.X) && AreDoublesEqual(vertex.Y,point.Y)) {
					return true;
        		}
        	}
			return false;
        }
        #endregion

        public override string ToString()
		{
			return Environment.NewLine + base.ToString();
		}

        protected void Contour_OnPropertyChanged(object sender, PropertyChangedEventArgs args) {
            CalcProperties();
            OnPropertyChanged();
        }

        protected void Contour_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            //CalcProperties();
            //();
        }
    }
}
