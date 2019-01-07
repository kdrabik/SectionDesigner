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
        private PlotModel _SectionPlotModel;
        public PlotModel SectionPlotModel {
            get { return _SectionPlotModel; }
            set {
                _SectionPlotModel = value;
                OnPropertyChanged();
            }
        }

        private TS_section _Section;
        public TS_section Section {
            get { return _Section; }
            set {
                if (_Section != null) {
                    _Section.ParametersChanged -= PlotView_PropertyChanged;
                }
                _Section = value;
                if (_Section != null) {
                    _Section.ParametersChanged += PlotView_PropertyChanged;
                }
                DrawSection();
                OnPropertyChanged();
            }
        }

        double generalThickness = 2.0;

        private void SetUpGraph(PlotModel sectionPlotModel) {
            sectionPlotModel.PlotType = PlotType.XY;
            sectionPlotModel.Axes.Clear();
            sectionPlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis());
            sectionPlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis());
            sectionPlotModel.Axes[0].Position = OxyPlot.Axes.AxisPosition.Left;
            sectionPlotModel.Axes[1].Position = OxyPlot.Axes.AxisPosition.Bottom;
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

        public OxyPlotViewModel() {}

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

        private void DrawSection() {
            PlotModel sectionPlotModel = new PlotModel();
            bool czydobry = Section.Parts[0].IsCorrect;
            SectionPlotModel = new PlotModel();
            SetUpGraph(sectionPlotModel);

            Random rand = new Random();
            byte[] byteColor = new byte[3];

            foreach (var part in Section.Parts) {

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
                sectionPlotModel.Series.Add(seria);

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
                    sectionPlotModel.Series.Add(seria1);
                }
            }

            foreach (var reoGroup in Section.Reinforcement) {

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
                sectionPlotModel.Series.Add(seria);
            }

            {
                OxyColor randomColor;
                rand.NextBytes(byteColor);
                randomColor = OxyColor.FromArgb(255, byteColor[0], byteColor[1], byteColor[2]);

                ScatterSeries seria = new ScatterSeries();
                seria.MarkerStroke = randomColor;
                seria.MarkerType = MarkerType.Plus;
                seria.MarkerStrokeThickness = 3.5;

                ScatterPoint point = new ScatterPoint(Section.Centroid.X, Section.Centroid.Y);
                point.Size = 10;
                seria.Points.Add(point);

                seria.Title = "Section centroid";
                sectionPlotModel.Series.Add(seria);

                //FunctionSeries functionSeries1 = new FunctionSeries();
                // dopracowac jeszcze to

                foreach (var part in Section.Parts) {
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
                    sectionPlotModel.Series.Add(seria1);
                }
            }

            SectionPlotModel = sectionPlotModel;
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
            DrawSection();
        }
        #endregion
    }
}
