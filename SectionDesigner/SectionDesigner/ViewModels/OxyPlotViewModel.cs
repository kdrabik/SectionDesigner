using System.Net.NetworkInformation;
using OxyPlot;
using System.Collections.Generic;
using Majstersztyk;
using System.ComponentModel;
using System;
using OxyPlot.Series;
using System.Runtime.CompilerServices;

namespace SectionDesigner.ViewModels
{
    public class OxyPlotViewModel : INotifyPropertyChanged
    {
    	public OxyPlotViewModel(){    	}
    	
        private PlotModel _SectionPlotModel;
        public PlotModel SectionPlotModel {
            get { return _SectionPlotModel; }
            set {
                _SectionPlotModel = value;
                OnPropertyChanged(); } }

        private TS_section _Section;
		public TS_section Section {
			get { return _Section; }
			set {
				if (_Section != null) {
					_Section.ParametersChanged -= PlotView_PropertyChanged;
					_Section.SelectedMemberChanged -= SelectedContour_PropertyChanged;
 
					foreach (var Part in _Section.Parts) Part.SelectedMemberChanged -= SelectedContour_PropertyChanged;
					foreach (var Reo in _Section.Reinforcement) Reo.SelectedMemberChanged -= SelectedContour_PropertyChanged;
				}
 
				_Section = value;
 
				if (_Section != null) {
					_Section.ParametersChanged += PlotView_PropertyChanged;
					_Section.SelectedMemberChanged += SelectedContour_PropertyChanged;
 
					foreach (var Part in _Section.Parts) Part.SelectedMemberChanged += SelectedContour_PropertyChanged;
					foreach (var Reo in _Section.Reinforcement) Reo.SelectedMemberChanged += SelectedContour_PropertyChanged;
				}
 
				SectionPlotModel = DrawSection();
				OnPropertyChanged();
			}
		}
			
		
        
		List<Series> _currentSeries;
        
        private TS_region _SelectedContour;
		public TS_region SelectedContour {
			get{ return _SelectedContour; }
			set {_SelectedContour = value;}
		}
        		
        		//DrawSelectedMember(_SelectedContour);
        		//OnPropertyChanged();
        		//OnPropertyChanged("SectionPlotModel"); } }
        
		private List<OxyColor> Kolory_Parts = new List<OxyColor>() { 
			OxyColors.Green, OxyColors.Blue, OxyColors.Red, OxyColors.Yellow, OxyColors.Purple, OxyColors.Aqua};
		
        private List<OxyColor> Kolory_Reinforcement = new List<OxyColor>() {
        	OxyColors.Gray, OxyColors.DarkSlateGray, OxyColors.DarkGray, OxyColors.SlateGray, OxyColors.DimGray, OxyColors.LightSlateGray };
		
		private List<OxyColor> Kolory_Centroids = new List<OxyColor>() {
			OxyColors.DarkGreen, OxyColors.DarkBlue, OxyColors.DarkRed, OxyColors.DarkOrange, OxyColors.DarkMagenta, OxyColors.DarkSeaGreen};
        
        double generalThickness = 2.0;
        
        private void SetUpGraph(PlotModel sectionPlotModel) {
            sectionPlotModel.PlotType = PlotType.XY;
            sectionPlotModel.Axes.Clear();
            sectionPlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis());
            sectionPlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis());
            sectionPlotModel.Axes[0].Position = OxyPlot.Axes.AxisPosition.Left;
            sectionPlotModel.Axes[1].Position = OxyPlot.Axes.AxisPosition.Bottom;

            foreach (var axis in sectionPlotModel.Axes) {
                axis.Layer = OxyPlot.Axes.AxisLayer.AboveSeries;
                axis.PositionAtZeroCrossing = true;
                axis.AxislineStyle = LineStyle.Solid;

                axis.TickStyle = OxyPlot.Axes.TickStyle.Crossing;

                axis.MajorGridlineStyle = LineStyle.Dot;
                axis.MajorGridlineThickness = 1;

                axis.MinorGridlineStyle = LineStyle.Dot;
                axis.MinorGridlineThickness = 0.75;

            }

            sectionPlotModel.LegendPosition = LegendPosition.BottomCenter;
            sectionPlotModel.LegendOrientation = LegendOrientation.Horizontal;
            sectionPlotModel.LegendPlacement = LegendPlacement.Outside;

            sectionPlotModel.LegendSymbolLength = 40;
        }

        private PlotModel DrawSection() {
			PlotModel sectionPlotModel = new PlotModel();
			SetUpGraph(sectionPlotModel);
			
			// RYSOWANIE KOLEJNYCH PARTÓW
			foreach (var part in Section.Parts) {

				AreaSeries seria_part = new AreaSeries();
				seria_part.Color = Kolory_Parts[Section.Parts.IndexOf(part)];
				seria_part.StrokeThickness = generalThickness;

				// KONTUR PARTU
				foreach (var side in part.Contour.Sides) {
					DataPoint point1 = new DataPoint(side.StartPoint.X, side.StartPoint.Y);
					seria_part.Points.Add(point1);
					DataPoint point2 = new DataPoint(side.EndPoint.X, side.EndPoint.Y);
					seria_part.Points.Add(point2);
				}
				seria_part.Title = part.Name;
				sectionPlotModel.Series.Add(seria_part);

				// VOIDY W PARCIE
				foreach (var _void in part.Voids) {
					AreaSeries seria_void = new AreaSeries();
					seria_void.Color = Kolory_Parts[Section.Parts.IndexOf(part)];
					seria_void.Fill = OxyColors.White;
					seria_void.StrokeThickness = generalThickness;

					foreach (var side in _void.Sides) {
						DataPoint point1 = new DataPoint(side.StartPoint.X, side.StartPoint.Y);
						seria_void.Points.Add(point1);
						DataPoint point2 = new DataPoint(side.EndPoint.X, side.EndPoint.Y);
						seria_void.Points.Add(point2);
					}
					sectionPlotModel.Series.Add(seria_void);
				}
			}

			// RYSOWANIE KOLEJNYCH GRUP ZBROJENIA
			foreach (var reoGroup in Section.Reinforcement) {
				ScatterSeries seria_reo = new ScatterSeries();
				seria_reo.MarkerFill = Kolory_Reinforcement[Section.Reinforcement.IndexOf(reoGroup)];
				seria_reo.MarkerType = MarkerType.Circle;
                
				foreach (var bar in reoGroup.Bars) {
					ScatterPoint point = new ScatterPoint(bar.Coordinates.X, bar.Coordinates.Y);
					point.Size = bar.Diameter * 100;
					seria_reo.Points.Add(point);
				}
				seria_reo.Title = reoGroup.Name;
				sectionPlotModel.Series.Add(seria_reo);
			}
			
			//RYSOWANIE ŚRODKA CIĘŻKOŚCI CAŁEGO ZBROJENIA			
			ScatterSeries seria_centroid0 = new ScatterSeries();
			seria_centroid0.MarkerStroke = OxyColors.Black;
			seria_centroid0.MarkerType = MarkerType.Plus;
			seria_centroid0.MarkerStrokeThickness = 3.5;

			ScatterPoint pointC0 = new ScatterPoint(Section.Centroid.X, Section.Centroid.Y);
			pointC0.Size = 10;
			seria_centroid0.Points.Add(pointC0);

			seria_centroid0.Title = "Section centroid";
			sectionPlotModel.Series.Add(seria_centroid0);
			
			//RYSOWANIE GŁÓWNYCH OSI BEZWŁADNOŚCI PRZEKROJU
			
			
			//RYSOWANIE ŚRODKÓW CIĘŻKOŚCI PARTÓW
			foreach (var part in Section.Parts) {
				ScatterSeries seria_centroids = new ScatterSeries();
				seria_centroids.MarkerStroke = Kolory_Centroids[Section.Parts.IndexOf(part)];
				seria_centroids.MarkerType = MarkerType.Plus;
				seria_centroids.MarkerStrokeThickness = 2.5;

				ScatterPoint pointC = new ScatterPoint(part.Centroid.X, part.Centroid.Y);
				pointC.Size = 7.5;
				seria_centroids.Points.Add(pointC);

				seria_centroids.Title = part.Name + " - centroid";
				sectionPlotModel.Series.Add(seria_centroids);
			}

			//PRZYPISANIE MODELU
			return sectionPlotModel;			
		}
        
        private List<Series> DrawSelectedMember(TS_region selectedMember)
        {
			if (selectedMember != null) SelectedContour = selectedMember;
			else return null;
        	
			OxyColor selectedColor = OxyColors.Magenta;
			
        	if ((selectedMember as TS_part) != null) {
        		TS_contour contour = ((selectedMember as TS_part).SelectedMember as TS_contour);
        		AreaSeries seria_cont = new AreaSeries();
				seria_cont.Color = selectedColor;
				seria_cont.StrokeThickness = 2 * generalThickness;
			
        		foreach (var side in contour.Sides) {
					DataPoint point1 = new DataPoint(side.StartPoint.X, side.StartPoint.Y);
					seria_cont.Points.Add(point1);
					DataPoint point2 = new DataPoint(side.EndPoint.X, side.EndPoint.Y);
					seria_cont.Points.Add(point2);
				}
				
				TS_contour contour2 = (selectedMember as TS_part).Contour;
        		LineSeries seria_part = new LineSeries();
				seria_part.Color = selectedColor;
				seria_part.StrokeThickness = 2 * generalThickness;
				seria_part.LineStyle = LineStyle.Dash;
			
        		foreach (var side in contour2.Sides) {
					DataPoint point1 = new DataPoint(side.StartPoint.X, side.StartPoint.Y);
					seria_part.Points.Add(point1);
					DataPoint point2 = new DataPoint(side.EndPoint.X, side.EndPoint.Y);
					seria_part.Points.Add(point2);
				}
				
				//SectionPlotModel.Series.Add(seria_cont);
				//SectionPlotModel.Series.Add(seria_part);
				
				return new List<Series>(){ seria_cont, seria_part};
				//_currentSeries.Add = seria_part;
				
        	}
			
			if ((selectedMember as TS_contour) != null) {
        		TS_contour contour = selectedMember as TS_contour;
        		AreaSeries seria_cont = new AreaSeries();
				seria_cont.Color = selectedColor;
				seria_cont.StrokeThickness = 2 * generalThickness;
			
        		foreach (var side in contour.Sides) {
					DataPoint point1 = new DataPoint(side.StartPoint.X, side.StartPoint.Y);
					seria_cont.Points.Add(point1);
					DataPoint point2 = new DataPoint(side.EndPoint.X, side.EndPoint.Y);
					seria_cont.Points.Add(point2);
				}
								
				//SectionPlotModel.Series.Add(seria_cont);
				
				//_currentSeries.Add = seria_part;
				return new List<Series>(){ seria_cont};
        	}
        	// to poniżej na pewno do poprawy
        	if ((selectedMember as TS_part).SelectedMember != null) {
				TS_reinforcement reo = selectedMember as TS_reinforcement;
				
				ScatterSeries seria_reo = new ScatterSeries();
				seria_reo.MarkerFill = selectedColor;
				seria_reo.MarkerType = MarkerType.Circle;
                
				foreach (var bar in reo.Bars) {
					ScatterPoint point = new ScatterPoint(bar.Coordinates.X, bar.Coordinates.Y);
					point.Size = bar.Diameter * 100;
					seria_reo.Points.Add(point);
				}
				//SectionPlotModel.Series.Add(seria_reo);
				
				//_currentSeries.Add = seria_reo;
				return new List<Series>() { seria_reo };
			}
			return new List<Series>();
        }
        
		private void UpdateGraph()
		{				
			PlotModel model = DrawSection();
			List<Series> series = DrawSelectedMember(SelectedContour);
			//_currentSeries = series;
			foreach (var seria in series) {
				model.Series.Add(seria);
			}
			SectionPlotModel = model;
		}
        
        #region InotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName="") {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void PlotView_PropertyChanged(object sender, EventArgs e) {
            UpdateGraph();
        }
        
        private void SelectedContour_PropertyChanged(object sender, EventArgs e) {
			SelectedContour = sender as TS_region;
			if (SelectedContour != null) {
				/*
        		if (_currentSeries != null)
        			foreach (var serie in _currentSeries) {
						SectionPlotModel.Series.Remove(serie); }
				*/
				UpdateGraph();
			}
        }
        #endregion
    }
}
