using DIgitalWallet.Commmon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using IServiceCollection = Microsoft.Extensions.DependencyInjection.IServiceCollection;
namespace DigitalWallet.Commmon
{
    public static class DIRegister
    {
        public class SerilogService : ISerilogService
        {
            private readonly ILogger<SerilogService> _logger;

            public SerilogService(ILogger<SerilogService> logger)
            {
                _logger = logger;
            }

            public void CustomLog(LogLevel logLevel, string source, string serviceName, int line = 0, string? userId = "", string? description = "", object? data = null, string? exception = "")
            {
                string message = $"{description}, User: {userId}, Data: {data}";
                switch (logLevel)
                {
                    case LogLevel.Information:
                        _logger.LogInformation(message);
                        break;
                    case LogLevel.Warning:
                        _logger.LogWarning(message);
                        break;
                    case LogLevel.Error:
                        _logger.LogError(message);
                        break;
                    default:
                        _logger.LogInformation(message); // Default logging
                        break;
                }
            }
        }

        public static IServiceCollection AddCommonInjections(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ISerilogService, SerilogService>();

            var seqServerUrl = configuration["Seq:localSeqServer"];

            if (string.IsNullOrEmpty(seqServerUrl))
            {
                throw new ArgumentException("The Seq server URL is not configured properly.");
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(seqServerUrl)
                .CreateLogger();

            return services;
        }
    }

    public interface ISerilogService
    {
        void CustomLog(LogLevel logLevel, string source, string serviceName, int line = 0, string? userId = "", string? description = "", object? data = null, string? exception = "");
    }
}
