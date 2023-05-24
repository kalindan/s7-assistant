using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ScottPlot;

namespace S7Assistant;
public class LivePlot
{

    public bool LiveTraceOn { get; set; }
    private FormsPlotViewer? _plotViewer;
    private Plot _plt;
    private readonly ILogger<LivePlot> _logger;
    public delegate Task HandleRender();

    public LivePlot(ILogger<LivePlot> logger)
    {
        _logger = logger;
        _plt = new Plot(400, 300);
        Rendered += RenderHandler;
    }
    public void ShowPlot()
    {
        _plotViewer = new FormsPlotViewer(_plt, windowTitle: "S7 Tracer");
        _plotViewer.ShowDialog();
    }
    public async Task RenderHandler()
    {
        await Task.Run(() => _plotViewer?.formsPlot1.Render());
    }
    public event HandleRender Rendered;
    public async Task LivePlotAsync<T>(List<T> traceBuffer, int samplingRate) where T : class
    {
        Type? type = typeof(T);
        PropertyInfo[] props = type.GetProperties();
        foreach (var prop in props)
        {
            Console.WriteLine(prop.Name);
        }
        LiveTraceOn = true;
        List<double> axisX = new();
        List<double> axisY = new();
        _logger.LogInformation("Initialized");
        while (LiveTraceOn)
        {
            if (traceBuffer.Count() > 0)
            {
                axisX.Clear();
                axisY.Clear();
                for (int i = 0; i < traceBuffer.Count(); i++)
                {
                    axisX.Add((double)i);
                    axisY.Add(Convert.ToDouble(props[0].GetValue(traceBuffer[i])));
                }
                _plt.Clear();
                _plt.AddSignal(axisY.ToArray(), sampleRate: 1000 / samplingRate);
                _plt.Render();
                Rendered?.Invoke();
                await Task.Delay(samplingRate);
            }
        }
    }
}
