using System.Collections.Generic;
using System.Linq;

namespace CodeTracker.Models
{
    public class ChartDataPoint
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; }
    }

    public static class ChartColors
    {
        public static readonly string[] DefaultColors = new[]
        {
            "#2196F3", "#4CAF50", "#FFC107", "#FF5722", "#9C27B0",
            "#00BCD4", "#FF9800", "#E91E63", "#3F51B5", "#009688",
            "#8BC34A", "#CDDC39", "#FF6F00", "#795548", "#607D8B"
        };

        public static List<ChartDataPoint> CreateChartData(Dictionary<string, double> data)
        {
            var total = data.Values.Sum();
            if (total == 0) return new List<ChartDataPoint>();

            var result = new List<ChartDataPoint>();
            int colorIndex = 0;

            foreach (var item in data.OrderByDescending(x => x.Value))
            {
                result.Add(new ChartDataPoint
                {
                    Label = item.Key,
                    Value = item.Value,
                    Percentage = (item.Value / total) * 100,
                    Color = DefaultColors[colorIndex % DefaultColors.Length]
                });
                colorIndex++;
            }

            return result;
        }
    }
}
