using OxyPlot;
using System.Collections.Generic;
using Majstersztyk;
using System.ComponentModel;
using System;
using OxyPlot.Series;

namespace SectionDesigner.ViewModels
{
    public class OxyPlotViewModel : INotifyPropertyChanged
    {
        private PlotModel _SectionPlotModel;

        public PlotModel SectionPlotModel {
            get { return _SectionPlotModel; }
            set {
                _SectionPlotModel = value;
                OnPropertyChanged("SectionPlotModel");
            }
        }

        double generalThickness = 2.0;

        private void SetUpGraph() {
            SectionPlotModel.PlotType = PlotType.XY;
            SectionPlotModel.Axes.Clear();
            SectionPlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis());
            SectionPlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis());
            SectionPlotModel.Axes[0].Position = OxyPlot.Axes.AxisPosition.Left;
            SectionPlotModel.Axes[1].Position = OxyPlot.Axes.AxisPosition.Bottom;
            /*SectionPlotModel.Axes[0].PositionAtZeroCrossing = true;
            SectionPlotModel.Axes[1].PositionAtZeroCrossing = true;
            SectionPlotModel.Axes[0].AxislineStyle = LineStyle.Solid;
            SectionPlotModel.Axes[1].AxislineStyle = LineStyle.Solid;
            //SectionPlotModel.Axes[0].TickStyle = OxyPlot.Axes.TickStyle.Inside;
            //SectionPlotModel.Axes[1].TickStyle = OxyPlot.Axes.TickStyle.Outside;
            SectionPlotModel.Axes[0].MajorGridlineStyle = LineStyle.Dot;
            SectionPlotModel.Axes[1].MajorGridlineStyle = LineStyle.Dot;
            SectionPlotModel.Axes[0].MajorGridlineThickness = 1;
            SectionPlotModel.Axes[1].MajorGridlineThickness = 1;
            SectionPlotModel.Axes[0].MinorGridlineStyle = LineStyle.Dot;
            SectionPlotModel.Axes[1].MinorGridlineStyle = LineStyle.Dot;
            SectionPlotModel.Axes[0].MinorGridlineThickness = 0.75;
            SectionPlotModel.Axes[1].MinorGridlineThickness = 0.75;*/

            foreach (var axis in SectionPlotModel.Axes) {
                axis.Layer = OxyPlot.Axes.AxisLayer.AboveSeries;
                axis.PositionAtZeroCrossing = true;
                axis.AxislineStyle = LineStyle.Solid;

                axis.TickStyle = OxyPlot.Axes.TickStyle.Crossing;

                axis.MajorGridlineStyle = LineStyle.Dot;
                axis.MajorGridlineThickness = 1;

                axis.MinorGridlineStyle = LineStyle.Dot;
                axis.MinorGridlineThickness = 0.75;

            }

            SectionPlotModel.LegendPosition = LegendPosition.BottomCenter;
            SectionPlotModel.LegendOrientation = LegendOrientation.Horizontal;
            SectionPlotModel.LegendPlacement = LegendPlacement.Outside;

            SectionPlotModel.LegendSymbolLength = 40;
            
        }

        public OxyPlotViewModel() {
            SectionPlotModel = new PlotModel();
            SetUpGraph();
        }

        /*public void AddPart(TS_part part) {
            OxyPlot.Series.LineSeries seria = new OxyPlot.Series.LineSeries();
            seria.Color = OxyColors.Blue;
            seria.StrokeThickness = 2.0;
            
            foreach (var side in part.Contour.Sides) {
                DataPoint point1 = new DataPoint(side.StartPoint.X, side.StartPoint.Y);
                seria.Points.Add(point1);
                DataPoint point2 = new DataPoint(side.EndPoint.X, side.EndPoint.Y);
                seria.Points.Add(point2);
            }
            SectionPlotModel.Series.Add(seria);
            
            foreach (var _void in part.Voids) {
                OxyPlot.Series.LineSeries seria1 = new OxyPlot.Series.LineSeries();
                seria1.Color = OxyColors.Blue;
                seria1.StrokeThickness = 2.0;

                foreach (var side in _void.Sides) {
                    DataPoint point1 = new DataPoint(side.StartPoint.X, side.StartPoint.Y);
                    seria1.Points.Add(point1);
                    DataPoint point2 = new DataPoint(side.EndPoint.X, side.EndPoint.Y);
                    seria1.Points.Add(point2);
                }
                SectionPlotModel.Series.Add(seria1);
            }
        }
        */

        public void DrawSection(TS_section section) {

            Random rand = new Random();
            byte[] byteColor = new byte[3];

            foreach (var part in section.Parts) {

                OxyColor randomColor;
                rand.NextBytes(byteColor);
                randomColor = OxyColor.FromArgb(255, byteColor[0], byteColor[1], byteColor[2]);

                AreaSeries seria = new AreaSeries();
                seria.Color = randomColor;
                seria.StrokeThickness = generalThickness;

                foreach (var side in part.Contour.Sides) {
                    DataPoint point1 = new DataPoint(side.StartPoint.X, side.StartPoint.Y);
                    seria.Points.Add(point1);
                    DataPoint point2 = new DataPoint(side.EndPoint.X, side.EndPoint.Y);
                    seria.Points.Add(point2);
                }
                seria.Title = part.Name;
                SectionPlotModel.Series.Add(seria);

                foreach (var _void in part.Voids) {
                    AreaSeries seria1 = new AreaSeries();
                    seria1.Color = randomColor;
                    seria1.Fill = OxyColors.White;
                    seria1.StrokeThickness = generalThickness;

                    foreach (var side in _void.Sides) {
                        DataPoint point1 = new DataPoint(side.StartPoint.X, side.StartPoint.Y);
                        seria1.Points.Add(point1);
                        DataPoint point2 = new DataPoint(side.EndPoint.X, side.EndPoint.Y);
                        seria1.Points.Add(point2);
                    }
                    //seria1.Title = _void.Name;
                    SectionPlotModel.Series.Add(seria1);
                }
            }

            foreach (var reoGroup in section.Reinforcement) {

                OxyColor randomColor;
                rand.NextBytes(byteColor);
                randomColor = OxyColor.FromArgb(255, byteColor[0], byteColor[1], byteColor[2]);

                ScatterSeries seria = new ScatterSeries();
                seria.MarkerFill = randomColor;
                seria.MarkerType = MarkerType.Circle;
                
                foreach (var bar in reoGroup.Bars) {
                    ScatterPoint point = new ScatterPoint(bar.Coordinates.X, bar.Coordinates.Y);
                    point.Size = bar.Diameter*100;
                    seria.Points.Add(point);
                }
                seria.Title = reoGroup.Name;
                SectionPlotModel.Series.Add(seria);
            }

            DrawCentroid(section);
        }
        
        private void DrawCentroid(TS_section section) {

            Random rand = new Random();
            byte[] byteColor = new byte[3];

            OxyColor randomColor;
            rand.NextBytes(byteColor);
            randomColor = OxyColor.FromArgb(255, byteColor[0], byteColor[1], byteColor[2]);

            ScatterSeries seria = new ScatterSeries();
            seria.MarkerStroke = randomColor;
            seria.MarkerType = MarkerType.Plus;
            seria.MarkerStrokeThickness = 3.5;

            ScatterPoint point = new ScatterPoint(section.Centroid.X, section.Centroid.Y);
            point.Size = 10;
            seria.Points.Add(point);

            seria.Title = "Section centroid";
            SectionPlotModel.Series.Add(seria);

            //FunctionSeries functionSeries1 = new FunctionSeries();
            // dopracowac jeszcze to

            foreach (var part in section.Parts) {
                rand.NextBytes(byteColor);
                randomColor = OxyColor.FromArgb(255, byteColor[0], byteColor[1], byteColor[2]);

                ScatterSeries seria1 = new ScatterSeries();
                seria1.MarkerStroke = randomColor;
                seria1.MarkerType = MarkerType.Plus;
                seria1.MarkerStrokeThickness = 2.5;

                ScatterPoint point1 = new ScatterPoint(part.Centroid.X, part.Centroid.Y);
                point1.Size = 7.5;
                seria1.Points.Add(point1);

                seria1.Title = part.Name + " - centroid";
                SectionPlotModel.Series.Add(seria1);
            }
        }

        #region InotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
