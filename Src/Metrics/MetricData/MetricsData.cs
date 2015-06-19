
using System;
using System.Collections.Generic;
using System.Linq;

namespace Metrics.MetricData
{
    public sealed class MetricsData
    {
        public static readonly MetricsData Empty = new MetricsData(string.Empty, DateTime.MinValue,
            Enumerable.Empty<EnvironmentEntry>(),
            Enumerable.Empty<GaugeValueSource>(),
            Enumerable.Empty<CounterValueSource>(),
            Enumerable.Empty<MeterValueSource>(),
            Enumerable.Empty<HistogramValueSource>(),
            Enumerable.Empty<TimerValueSource>(),
            Enumerable.Empty<MetricsData>(),
            Enumerable.Empty<BarGaugeValueSource>());

        public readonly string Context;
        public readonly DateTime Timestamp;

        public readonly IEnumerable<EnvironmentEntry> Environment;

        public readonly IEnumerable<GaugeValueSource> Gauges;
        public readonly IEnumerable<CounterValueSource> Counters;
        public readonly IEnumerable<MeterValueSource> Meters;
        public readonly IEnumerable<HistogramValueSource> Histograms;
        public readonly IEnumerable<TimerValueSource> Timers;
        public readonly IEnumerable<MetricsData> ChildMetrics;
        public readonly IEnumerable<BarGaugeValueSource> BarGauges;

        public MetricsData(string context, DateTime timestamp,
            IEnumerable<EnvironmentEntry> environment,
            IEnumerable<GaugeValueSource> gauges,
            IEnumerable<CounterValueSource> counters,
            IEnumerable<MeterValueSource> meters,
            IEnumerable<HistogramValueSource> histograms,
            IEnumerable<TimerValueSource> timers,
            IEnumerable<MetricsData> childMetrics,
            IEnumerable<BarGaugeValueSource> barGauges)
        {
            this.Context = context;
            this.Timestamp = timestamp;
            this.Environment = environment;
            this.Gauges = gauges;
            this.Counters = counters;
            this.Meters = meters;
            this.Histograms = histograms;
            this.Timers = timers;
            this.ChildMetrics = childMetrics;
            this.BarGauges = barGauges;
        }

        public MetricsData Filter(MetricsFilter filter)
        {
            if (!filter.IsMatch(this.Context))
            {
                return MetricsData.Empty;
            }

            return new MetricsData(this.Context, this.Timestamp,
                this.Environment,
                this.Gauges.Where(filter.IsMatch),
                this.Counters.Where(filter.IsMatch),
                this.Meters.Where(filter.IsMatch),
                this.Histograms.Where(filter.IsMatch),
                this.Timers.Where(filter.IsMatch),
                this.ChildMetrics.Select(m => m.Filter(filter)),
                this.BarGauges.Where(filter.IsMatch));
        }

        public MetricsData Flatten()
        {
            return new MetricsData(this.Context, this.Timestamp,
                this.Environment.Union(this.ChildMetrics.SelectMany(m => m.Flatten().Environment)),
                this.Gauges.Union(this.ChildMetrics.SelectMany(m => m.Flatten().Gauges)),
                this.Counters.Union(this.ChildMetrics.SelectMany(m => m.Flatten().Counters)),
                this.Meters.Union(this.ChildMetrics.SelectMany(m => m.Flatten().Meters)),
                this.Histograms.Union(this.ChildMetrics.SelectMany(m => m.Flatten().Histograms)),
                this.Timers.Union(this.ChildMetrics.SelectMany(m => m.Flatten().Timers)),
                Enumerable.Empty<MetricsData>(),
                this.BarGauges.Union(this.ChildMetrics.SelectMany(m => m.Flatten().BarGauges))
            );
        }

        public MetricsData OldFormat()
        {
            return OldFormat(string.Empty);
        }

        private MetricsData OldFormat(string prefix)
        {
            var gauges = this.Gauges
                .Select(g => new GaugeValueSource(FormatName(prefix, g.Name), g.ValueProvider, g.Unit, g.Tags))
                .Union(this.ChildMetrics.SelectMany(m => m.OldFormat(FormatPrefix(prefix, m.Context)).Gauges));

            var counters = this.Counters
                .Select(c => new CounterValueSource(FormatName(prefix, c.Name), c.ValueProvider, c.Unit, c.Tags))
                .Union(this.ChildMetrics.SelectMany(m => m.OldFormat(FormatPrefix(prefix, m.Context)).Counters));

            var meters = this.Meters
                .Select(m => new MeterValueSource(FormatName(prefix, m.Name), m.ValueProvider, m.Unit, m.RateUnit, m.Tags))
                .Union(this.ChildMetrics.SelectMany(m => m.OldFormat(FormatPrefix(prefix, m.Context)).Meters));

            var histograms = this.Histograms
                .Select(h => new HistogramValueSource(FormatName(prefix, h.Name), h.ValueProvider, h.Unit, h.Tags))
                .Union(this.ChildMetrics.SelectMany(m => m.OldFormat(FormatPrefix(prefix, m.Context)).Histograms));

            var timers = this.Timers
                .Select(t => new TimerValueSource(FormatName(prefix, t.Name), t.ValueProvider, t.Unit, t.RateUnit, t.DurationUnit, t.Tags))
                .Union(this.ChildMetrics.SelectMany(m => m.OldFormat(FormatPrefix(prefix, m.Context)).Timers));

            var environment = this.Environment
                .Select(e => new EnvironmentEntry(FormatName(prefix, e.Name), e.Value))
                .Union(this.ChildMetrics.SelectMany(e => e.OldFormat(FormatPrefix(prefix, e.Context)).Environment));

            var barGauges = this.BarGauges
                .Select(s => new BarGaugeValueSource(FormatName(prefix, s.Name), s.ValueProvider, s.Unit, s.YMax, s.Tags))
                .Union(this.ChildMetrics.SelectMany(m => m.OldFormat(FormatPrefix(prefix, m.Context)).BarGauges));

            return new MetricsData(this.Context, this.Timestamp, environment, gauges, counters, meters, histograms, timers, Enumerable.Empty<MetricsData>(), barGauges);
        }

        private static string FormatPrefix(string prefix, string context)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                return context;
            }
            return prefix + " - " + context;
        }

        private static string FormatName(string prefix, string name)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                return name;
            }
            return string.Format("[{0}] {1}", prefix, name);
        }
    }
}
