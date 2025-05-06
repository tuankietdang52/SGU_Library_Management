using DotNetEnv;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System.Windows.Media;
using static OfficeOpenXml.ExcelErrorValue;

namespace SGULibraryManagement.Utilities
{
    public static class StatisticsUtility
    {
        public static PlotModel CreateHorizontalBarChart(string title,
                                                         string seriesTitle,
                                                         Dictionary<string, BarItem> values,
                                                         string xAxisTitle = "",
                                                         string yAxisTitle = "")
        {
            PlotModel model = new()
            {
                Title = title
            };

            var color = (SolidColorBrush) App.Instance!.Resources["AppThemeSecondary"];
            var series = new BarSeries()
            {
                Title = seriesTitle,
                FillColor = color.ParseToOxyColor(),
                XAxisKey = "x",
                YAxisKey = "y",
            };

            var categoryAxis = new CategoryAxis
            {
                Key = "y",
                Title = xAxisTitle,
                Position = AxisPosition.Bottom,
                AxisTitleDistance = 20,
                TitleFontWeight = FontWeights.Bold,
                IsZoomEnabled = false
            };

            foreach (var item in values.Keys) {
                series.Items.Add(values[item]);
                categoryAxis.Labels.Add(item);
            }

            model.Series.Add(series);

            model.Axes.Add(categoryAxis);
            model.Axes.Add(new LinearAxis
            {
                Key = "x",
                Title = yAxisTitle,
                Position = AxisPosition.Left,
                AxisTitleDistance = 20,
                TitleFontWeight = FontWeights.Bold,
                IsZoomEnabled = false
            });

            return model;
        }

        public static PlotModel CreatePieChart(string title, Dictionary<string, double> values)
        {
            var model = new PlotModel { Title = title };
            var series = new PieSeries
            {
                StrokeThickness = 1,
                AngleSpan = 360,
                StartAngle = 0,
                InsideLabelPosition = 0.8
            };

            foreach (var item in values.Keys)
            {
                if (values[item] == 0) continue;
                PieSlice slice = new(item, values[item])
                {
                    Fill = ColorUtility.Random().ParseToOxyColor()
                };

                series.Slices.Add(slice);
            }

            if (series.Slices.Count == 1) series.StrokeThickness = 0;

            model.Series.Add(series);
            return model;
        }

        public static PlotModel CreatePieChart(string title, List<PieSlice> values)
        {
            var model = new PlotModel { Title = title };
            var series = new PieSeries
            {
                StrokeThickness = 1,
                AngleSpan = 360,
                StartAngle = 0,
                InsideLabelPosition = 0.8,
            };

            foreach (var item in values)
            {
                if (item.Value == 0) continue;
                series.Slices.Add(item);
            }

            if (series.Slices.Count == 1) series.StrokeThickness = 0;

            model.Series.Add(series);
            return model;
        }

        public static PlotModel CreateDateLineChart(string title,
                                                    string seriesTitle,
                                                    IEnumerable<DataPoint> values,
                                                    string xAxisTitle = "",
                                                    string yAxisTitle = "",
                                                    string format = "",
                                                    DateTime? start = null,
                                                    DateTime? end = null)
        {
            PlotModel model = new() { Title = title };

            var color = (SolidColorBrush)App.Instance!.Resources["AppThemeSecondary"];
            var series = new LineSeries()
            {
                Title = seriesTitle,
                Color = color.ParseToOxyColor(),
                CanTrackerInterpolatePoints = false,
                TrackerFormatString = "{0}\n{1}: {2:dd/MM/yyyy}\n{3}: {4:0.###}",
                MarkerType = MarkerType.Circle
            };

            series.Points.AddRange(values);
            model.Series.Add(series);

            var dateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = format,
                Title = xAxisTitle,
                AxisTitleDistance = 20,
                TitleFontWeight = FontWeights.Bold,
                MajorGridlineStyle = LineStyle.Solid
            };

            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = yAxisTitle,
                IsZoomEnabled = false,
                AxisTitleDistance = 20,
                Minimum = 0,
                AbsoluteMinimum = 0,
                MajorStep = 1,
                MinorStep = 1,
                TitleFontWeight = FontWeights.Bold,
                MajorGridlineStyle = LineStyle.Solid
            };

            AdjustDatePosition(dateAxis, values, start, end);

            if (!values.Any())
            {
                valueAxis.Minimum = 0;
                valueAxis.Maximum = 10;
            }

            model.Axes.Add(dateAxis);
            model.Axes.Add(valueAxis);
            
            return model;
        }

        private static void AdjustDatePosition(DateTimeAxis dateAxis,
                                               IEnumerable<DataPoint> values, 
                                               DateTime? start, 
                                               DateTime? end)
        {
            int length = values.Count();

            if (length == 0)
            {
                if (!start.HasValue)
                {
                    if (end.HasValue) dateAxis.Minimum = DateTimeAxis.ToDouble(end.Value.AddDays(-10));
                    else dateAxis.Minimum = DateTimeAxis.ToDouble(DateTime.Today);
                }
                else dateAxis.Minimum = DateTimeAxis.ToDouble(start.Value);

                if (!end.HasValue)
                {
                    if (start.HasValue) dateAxis.Maximum = DateTimeAxis.ToDouble(start.Value.AddDays(10));
                    else dateAxis.Maximum = DateTimeAxis.ToDouble(DateTime.Today.AddDays(10));
                }
                else dateAxis.Maximum = DateTimeAxis.ToDouble(end.Value);
            }

            if (length == 1)
            {
                var data = values.First();
                dateAxis.Minimum = data.X - 2;
                dateAxis.Maximum = data.X + 2;
            }
        }
    }
}
