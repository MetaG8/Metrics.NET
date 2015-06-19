using System;
using Metrics.MetricData;
namespace Metrics.Core
{
    public interface BarGaugeImplementation : MetricValueProvider<double> { }

    public sealed class BarGaugeMetric : BarGaugeImplementation
    {
        private readonly Func<double> valueProvider;

        public BarGaugeMetric(Func<double> valueProvider)
        {
            this.valueProvider = valueProvider;
        }

        public double GetValue(bool resetMetric = false)
        {
            return this.Value;
        }

        public double Value
        {
            get
            {
                try
                {
                    return this.valueProvider();
                }
                catch (Exception x)
                {
                    MetricsErrorHandler.Handle(x, "Error executing Functional BarGauge");
                    return double.NaN;
                }
            }
        }
    }
}
