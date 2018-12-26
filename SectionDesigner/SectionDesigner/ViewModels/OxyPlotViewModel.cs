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
        public PlotModel SectionPlotModel { get; private set; }

        public OxyPlot.Series.LineSeries part = new OxyPlot.Series.LineSeries();

        public OxyPlotViewModel() {
            SectionPlotModel = new PlotModel();
            SectionPlotModel.LegendPosition = LegendPosition.BottomCenter;
            SectionPlotModel.Axes.Clear();
            SectionPlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis());
            SectionPlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis());
            SectionPlotModel.Axes[0].Position = OxyPlot.Axes.AxisPosition.Bottom;
            SectionPlotModel.Axes[1].Position = OxyPlot.Axes.AxisPosition.Left;
            SectionPlotModel.Axes[0].PositionAtZeroCrossing = true;
            SectionPlotModel.Axes[1].PositionAtZeroCrossing = true;
            SectionPlotModel.Axes[0].AxislineStyle = LineStyle.Automatic;
            SectionPlotModel.Axes[1].AxislineStyle = LineStyle.Automatic;
            
        }

        public void AddPart(TS_part part) {
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

        public void DrawSection(TS_section section) {

            double generalThickness = 2.0;
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
                SectionPlotModel.Series.Add(seria);

                foreach (var _void in part.Voids) {
                    AreaSeries seria1 = new AreaSeries();
                    seria1.Color = randomColor;
                    //seria1.Fill = OxyColors.White;
                    seria1.StrokeThickness = generalThickness;

                    foreach (var side in _void.Sides) {
                        DataPoint point1 = new DataPoint(side.StartPoint.X, side.StartPoint.Y);
                        seria1.Points.Add(point1);
                        DataPoint point2 = new DataPoint(side.EndPoint.X, side.EndPoint.Y);
                        seria1.Points.Add(point2);
                    }
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
                    ScatterPoint point = new ScatterPoint(bar.coordinates.X, bar.coordinates.Y);
                    point.Size = bar.Diameter*100;
                    seria.Points.Add(point);
                }
                SectionPlotModel.Series.Add(seria);
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
