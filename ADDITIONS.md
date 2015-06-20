This is a modified Version of etishor's fork of the Metrics.NET library.
For the documentation of the original project, please consult the [Metrics.NET Wiki](https://github.com/etishor/Metrics.NET/wiki).

## Additions/Changes:

BarGauge classes/interfaces added to provide a simple gauge to monitor the momentary status of a metric, for clarity purposes.

	//bargauge from Func<double>
	//ymax to set a fixed maximum value on the y-axis of the graph in the FlotVisualization app
	Metric.Bargauge(String name, Func<double> ValueProvider, Unit unit, double ymax)
	Metric.BarGauge("MyValue", () => ComputingMagicValue(). Unit.Costom("MyUnit"), 100)

The FlotVisualization app can now group metrics by Context. Context usage is described [here](https://github.com/etishor/Metrics.NET/wiki/Grouping-&-Organizing-Metrics).
In the FlotVisualization app (found at the Metric.Config  ` HttpEndpoint ` - default: ` http://localhost:1234/metrics `) this is represented by a drop-down menu.