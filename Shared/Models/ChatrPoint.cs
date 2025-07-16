namespace ZTACS.Shared.Models
{
    public class ChartPoint
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
        public string? Color { get; set; } // Optional: for customized coloring per point

        public DateTime? Timestamp { get; set; }

        // Optional: for tooltips or extended info
        public string? Tooltip { get; set; }

        // Optional: for categorization or grouping
        public string? Category { get; set; }

        // Constructor
        public ChartPoint()
        {
        }

        public ChartPoint(string label, double value, DateTime? TimeStamp = null, string? color = null, string? tooltip = null,
            string? category = null)
        {
            Timestamp = TimeStamp;
            Label = label;
            Value = value;
            Color = color;
            Tooltip = tooltip;
            Category = category;
        }
    }
}