
using System.Collections.Generic;
using Metrics.MetricData;

namespace Metrics.Json
{
    public class JsonBarGauge : JsonMetric
    {
        private double value;
        public double YMax { get; set; }

        public double? Value { get { return this.value; } set { this.value = value ?? double.NaN; } }

        public static JsonBarGauge FromBarGauge(BarGaugeValueSource bargauge)
        {
            return new JsonBarGauge
            {
                Name = bargauge.Name,
                Value = bargauge.Value,
                Unit = bargauge.Unit.Name,
                Tags = bargauge.Tags,
                YMax = bargauge.YMax
            };
        }

        public JsonObject ToJsonObject()
        {
            return new JsonObject(ToJsonProperties());
        }

        public IEnumerable<JsonProperty> ToJsonProperties()
        {
            yield return new JsonProperty("Name", this.Name);
            yield return new JsonProperty("Value", this.Value.Value);
            yield return new JsonProperty("Unit", this.Unit);
            yield return new JsonProperty("YMax", this.YMax);

            if (this.Tags.Length > 0)
            {
                yield return new JsonProperty("Tags", this.Tags);
            }
        }

        public BarGaugeValueSource ToValueSource()
        {
            return new BarGaugeValueSource(this.Name, ConstantValue.Provider(this.Value.Value), this.Unit, this.YMax, this.Tags);
        }
    }
}
