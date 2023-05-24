
namespace S7Assistant
{
    public interface IDBTracer<T> where T : class
    {
        public bool LiveTraceOn { get; set; }
        public List<T> TraceBuffer { get; set; }
        public Task<List<T>> TraceDBAsync(int db, int samplingTime, double duration);
        public List<T> TraceDB(int db, int samplingTime, double duration);
        public T SnapshotDB(int db);
        public Task<T> SnapshotDBAsync(int db);
        public Task LiveTraceDBAsync(int db, int samplingTime, int bufferSize);
    }
}