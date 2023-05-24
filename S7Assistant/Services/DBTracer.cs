using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace S7Assistant;

public class DBTracer<T> : IDBTracer<T> where T : class
{
    private IGateway _gateway;
    private readonly ILogger<DBTracer<T>> _logger;
    public bool LiveTraceOn { get; set; }
    public List<T> TraceBuffer { get; set; } = new();
    public DBTracer(ILogger<DBTracer<T>> logger, IGateway Gateway)
    {
        _logger = logger;
        _gateway = Gateway;
    }
    public async Task<List<T>> TraceDBAsync(int db, int samplingTime, double duration)
    {
        TimeSpan tspan = TimeSpan.FromSeconds(duration);
        DateTime startTime = DateTime.Now;
        List<T> tracedData = new();
        Stopwatch stopwatch = new();
        while (DateTime.Now - startTime < tspan)
        {
            stopwatch.Start();
            _logger.LogInformation("Tracing values at time {0}", DateTime.Now.ToString("HH:mm:ss.fff"));
            T resultDB = await _gateway.ReadDBAsync<T>(db);
            tracedData.Add(resultDB);
            stopwatch.Stop();
            await Task.Delay(samplingTime - (int)stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();
        }
        return tracedData;
    }
    public List<T> TraceDB(int db, int samplingTime, double duration)
    {
        TimeSpan tspan = TimeSpan.FromSeconds(duration);
        DateTime startTime = DateTime.Now;
        List<T> tracedData = new();
        Stopwatch stopwatch = new();
        while (DateTime.Now - startTime < tspan)
        {
            stopwatch.Start();
            _logger.LogInformation("Tracing values at time {0}", DateTime.Now.ToString("HH:mm:ss.fff"));
            T resultDB = _gateway.ReadDB<T>(db);
            tracedData.Add(resultDB);
            stopwatch.Stop();
            Thread.Sleep(samplingTime - (int)stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();
        }
        return tracedData;
    }
    public T SnapshotDB(int db)
    {
        return _gateway.ReadDB<T>(db);
    }
    public async Task<T> SnapshotDBAsync(int db)
    {
        return await _gateway.ReadDBAsync<T>(db);
    }
    public async Task LiveTraceDBAsync(int db, int samplingTime, int bufferSize)
    {
        _logger.LogInformation("Live trace of DB{0} started", db);
        TraceBuffer.Clear();
        Stopwatch stopwatch = new();
        LiveTraceOn = true;
        while (LiveTraceOn)
        {
            stopwatch.Start();
            _logger.LogInformation("Tracing values at time {0}", DateTime.Now.ToString("HH:mm:ss.fff"));
            T resultDB = await _gateway.ReadDBAsync<T>(db);
            TraceBuffer.Add(resultDB);
            if (TraceBuffer.Count() == bufferSize)
            {
                TraceBuffer.RemoveAt(0);
            }
            stopwatch.Stop();
            await Task.Delay(samplingTime - (int)stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();
        }
        _logger.LogInformation("Live trace of DB{0} was stopped", db);
    }
}
