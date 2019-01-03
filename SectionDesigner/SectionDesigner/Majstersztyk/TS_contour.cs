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
using System.ComponentModel;

namespace Majstersztyk
{
	/// <summary>
	/// Description of TS_contour.
	/// </summary>
	public class TS_contour:TS_region,INotifyPropertyChanged
    {
        private ObservableCollection<TS_point> _Vertices;

        public ObservableCollection<TS_point> Vertices {
            get { return _Vertices; }
            set { _Vertices = value;
                OnPropertyChanged("Vertices");
                CalcProperties();
            }
        }

        public ObservableCollection<TS_side> Sides { get; protected set; }
        public override string TypeOf { get { return typeOf; } }
        private new readonly string typeOf = "Contour";
        
        protected TS_contour() {
            _Vertices = new ObservableCollection<TS_point>();
            Sides = new ObservableCollection<TS_side>();
            Name = "Contour";
        }

        public TS_contour(ObservableCollection<TS_point> vertices) {
            Update(vertices);
            Name = "Contour";
        }

        public void Update(ObservableCollection<TS_point> vertices) {
            _Vertices = vertices;
            if (CalcArea() < 0)
            {
                List<TS_point> vert2 = new List<TS_point>(_Vertices);
                vert2.Reverse();
                _Vertices = new ObservableCollection<TS_point>(vert2);
            }

            Sides = GenerateSides(vertices);
            CalcProperties();
        }

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
        
        protected int IndexOfNextVertex(int currentVertex)
        {
            return (currentVertex + 1) % _Vertices.Count;
        }

        protected static ObservableCollection<TS_side> GenerateSides(ObservableCollection<TS_point> points)
        {
            ObservableCollection<TS_side> sides = new ObservableCollection<TS_side>();
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
			ObservableCollection<TS_point> horPoints = new ObservableCollection<TS_point>();
			
			foreach (var side in Sides) {
				TS_point pointX = side.CrossedPoint(horLine);
				if (pointX.X > point.X && !horPoints.Contains(pointX)) {
					horPoints.Add(pointX);
					}
				}
			
			int countH = horPoints.Count;
			if (countH % 2 != 0) return true;
			
			TS_line vertLine = new TS_line(1, 0, -point.X);
			ObservableCollection<TS_point> vertPoints = new ObservableCollection<TS_point>();
			
			foreach (var side in Sides) {
				TS_point pointY = side.CrossedPoint(vertLine);
				if (pointY.Y > point.Y && !vertPoints.Contains(pointY)) {
					vertPoints.Add(pointY);
				}
			}
			
			int countV = vertPoints.Count;
			if (countV % 2 != 0) return true;
			
			TS_line horLineL = new TS_line(0, point.Y);
			ObservableCollection<TS_point> horPointsL = new ObservableCollection<TS_point>();
			
			foreach (var side in Sides) {
				TS_point pointX = side.CrossedPoint(horLineL);
				if (pointX.X > point.X && !horPointsL.Contains(pointX)) {
					horPointsL.Add(pointX);
					}
				}
			
			int countHL = horPointsL.Count;
			if (countHL % 2 != 0) return true;
			
			ObservableCollection<TS_point> vertPointsD = new ObservableCollection<TS_point>();
			
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
        
		public override string ToString()
		{
			return Environment.NewLine + base.ToString();
		}



    }
}
