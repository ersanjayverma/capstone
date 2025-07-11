
namespace ZTACS.Shared.Models
{
    public class ChartPoint
    {
        public string Label { get; set; } = string.Empty;
        public decimal? Value { get; set; }
        public string? Color { get; set; }  // Optional: for customized coloring per point

        // Optional: for tooltips or extended info
        public string? Tooltip { get; set; }

        // Optional: for categorization or grouping
        public string? Category { get; set; }

        // Constructor
        public ChartPoint() { }

        public ChartPoint(string label, double value, string? color = null, string? tooltip = null, string? category = null)
        {
            Label = label;
            Value = value;
            Color = color;
            Tooltip = tooltip;
            Category = category;
        }
    }
}
