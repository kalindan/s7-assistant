using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace S7Assistant
{
    public class S7ServiceConfigurator<T> where T : class
    {
        private IServiceCollection _services { get; set; }
        public S7ServiceConfigurator(string ip)
        {
            _services = new ServiceCollection()
                .AddLogging(logging =>
                    {
                        logging
                        .ClearProviders()
                        .AddSerilog();
                        //.AddConsole();
                    })
                .AddSingleton<IGateway, Gateway>(services =>
                    {
                        var logger = services.GetRequiredService<ILogger<Gateway>>();
                        return new Gateway(ip, logger);
                    })
                .AddSingleton<IDBTracer<T>, DBTracer<T>>();
        }
        public IServiceProvider Build()
        {
            return _services.BuildServiceProvider();
        }
    }
}