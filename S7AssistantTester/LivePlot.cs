using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ScottPlot;

namespace S7AssistantTester;
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
    public async Task LivePlotAsync(List<Datablock> traceBuffer, int samplingRate)
    {
        LiveTraceOn = true;

        List<double> axisX = new();
        List<double> axisY = new();
        _logger.LogInformation("Initialized");
        int bufferLength = traceBuffer.Count();
        while (LiveTraceOn)
        {
            if (traceBuffer.Count() > 0)
            {
                axisX.Clear();
                axisY.Clear();
                bufferLength = traceBuffer.Count();
                for (int i = 0; i < traceBuffer.Count(); i++)
                {
                    axisX.Add((double)i);
                    axisY.Add((double)traceBuffer[i].FirstVar);
                }
                _plt.Clear();
                _plt.AddSignal(axisY.ToArray(), sampleRate: 1000 / samplingRate);
                _plt.Render();
                _logger.LogInformation(bufferLength.ToString());
                Rendered?.Invoke();
                await Task.Delay(samplingRate);
            }
        }
    }
}
