
namespace Metrics.MetricData
{
    /// <summary>
    /// Combines the value of a gauge (a double) with the defined unit for the value.
    /// </summary>
    public sealed class BarGaugeValueSource : MetricValueSource<double>
    {
        //private readonly double YMax;
        public BarGaugeValueSource(string name, MetricValueProvider<double> value, Unit unit, double ymax, MetricTags tags)
            : base(name, value, unit, tags)
        {
            this.YMax = ymax;
        }
        public double YMax { get; private set; }
    }
}
