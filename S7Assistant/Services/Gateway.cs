using System.Diagnostics;
using Microsoft.Extensions.Logging;
using S7.Net;

namespace S7Assistant;
public class Gateway : IGateway
{
    private Plc _plc;
    private readonly ILogger<Gateway> _logger;
    public string IP { get; set; }
    public Gateway(string ip, ILogger<Gateway> logger)
    {
        _plc = new Plc(CpuType.S71500, ip, 0, 1);
        _logger = logger;
        IP = ip;

        try
        {
            _plc.Open();
            _logger.LogInformation("PLC successfully connected");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }
    public T ReadDB<T>(int db) where T : class
    {
        var result = _plc.ReadClass<T>(db, 0);
        if (result == null)
        {
            throw new Exception("Null data from PLC");
        }
        return result;
    }
    public async Task<T> ReadDBAsync<T>(int db) where T : class
    {
        var result = await _plc.ReadClassAsync<T>(db, 0);
        if (result == null)
        {
            throw new Exception("Null data from PLC");
        }
        return result;
    }
}
