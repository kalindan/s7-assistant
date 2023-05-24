using System.Diagnostics;
using S7AssistantTester;
using S7Assistant;
using Microsoft.Extensions.DependencyInjection;
using ScottPlot;
using Microsoft.Extensions.Logging;
using Serilog;

Log.Logger = new LoggerConfiguration()
          .WriteTo.Console()
          .CreateLogger();

var services = new S7ServiceConfigurator<Datablock>("192.168.0.33").Build();
IDBTracer<Datablock> tracer = services.GetRequiredService<IDBTracer<Datablock>>();
LivePlot lplot = services.GetRequiredService<LivePlot>();

Task liveTrace = tracer.LiveTraceDBAsync(16, 150, 200);
Task livePlot = lplot.LivePlotAsync(tracer.TraceBuffer, 150);
lplot.ShowPlot();
await Task.WhenAll(livePlot, liveTrace);


Console.Read();
Console.WriteLine("Hello, World!");
