namespace S7Assistant
{
    public interface IGateway
    {
        public string IP { get; set; }
        public T ReadDB<T>(int db) where T : class;
        public Task<T> ReadDBAsync<T>(int db) where T : class;
    }
}