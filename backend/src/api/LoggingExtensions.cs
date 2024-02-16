using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace api;

public static class LoggingExtensions
{
    public static LoggerConfiguration WithCorrelationId(this LoggerEnrichmentConfiguration config)
    => config.With(new CorrelationIdEnricher());
}

public class CorrelationIdEnricher : ILogEventEnricher
{
    private const string _propertyName = "CorelationId";
    private readonly IHttpContextAccessor _contextAccessor;

    public CorrelationIdEnricher()
    {
        _contextAccessor = new HttpContextAccessor();
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _contextAccessor.HttpContext;
        if (httpContext == null)
        {
            return;
        }

        if (httpContext.Items[_propertyName] is LogEventProperty logEventProperty)
        {
            logEvent.AddPropertyIfAbsent(logEventProperty);
            return;
        }

        var correlationId = Guid.NewGuid().ToString();

        var correlationIdProperty = new LogEventProperty(_propertyName, new ScalarValue(correlationId));
        logEvent.AddOrUpdateProperty(correlationIdProperty);

        httpContext.Items.Add(_propertyName, correlationIdProperty);
    }
}
