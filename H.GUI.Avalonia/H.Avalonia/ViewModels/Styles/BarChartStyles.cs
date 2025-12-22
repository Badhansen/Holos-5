using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace H.Avalonia.ViewModels.Styles
{
    public class BarChartStyles
    {
        #region Properties

        //Sets the styles of the bars
        public readonly ColumnSeries<double> ColumnSeriesStyles = new ColumnSeries<double>
        {
            Fill = new LinearGradientPaint(
            new SKColor(155, 212, 106), new SKColor(83, 123, 58),
                new SKPoint(0.5f, 0),
                new SKPoint(0.5f, 1)),
        };

        //Sets the styles of the axis
        public static readonly Axis BarAxisStyles = new Axis
        {
            NamePaint = new SolidColorPaint(SKColors.Black),
            LabelsPaint = new SolidColorPaint(SKColors.Black),
            TextSize = 14,
            LabelsRotation = 20,
            ShowSeparatorLines = true,
            Padding = new LiveChartsCore.Drawing.Padding(-8, 10, 10, 0),
        };

        #endregion

        #region Public Methods

        public LinearGradientPaint SetColumnSeriesFill()
        {
            var fill = new LinearGradientPaint(new SKColor(155, 212, 106), new SKColor(83, 123, 58), new SKPoint(0.5f, 0), new SKPoint(0.5f, 1));
            return fill;
        }

        public Axis SetAxisStyling(string name, string[] labels)
        {
            var axis = new Axis
            {
                Name = name,
                NamePaint = new SolidColorPaint(SKColors.Black),
                LabelsPaint = new SolidColorPaint(SKColors.Black),
                TextSize = 14,
                LabelsRotation = 20,
                ShowSeparatorLines = true,
                Padding = new LiveChartsCore.Drawing.Padding(-8, 10, 10, 0),
                Labels = labels
            };

            return axis;
        }


        #endregion
    }
}
