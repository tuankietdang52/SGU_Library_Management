using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows.Media;

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
                Title = yAxisTitle,
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
                Title = xAxisTitle,
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
    }
}
