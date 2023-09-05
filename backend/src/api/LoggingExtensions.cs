using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace api;

public static class LoggingExtensions
{
    public static LoggerConfiguration WithCoretaltionId(this LoggerEnrichmentConfiguration config, string header, bool addIfMissing)
    => config.With(new CorelationIdEnricher(header, addIfMissing));
}

public class CorelationIdEnricher : ILogEventEnricher
{
    private const string CorrelationIdItemKey = "Serilog_CorrelationId";
    private const string PropertyName = "CorelationId";
    private readonly string _headerKey;
    private readonly bool _addValueIfHeaderAbsence;
    private readonly IHttpContextAccessor _contextAccessor;

    public CorelationIdEnricher(string headerKey, bool addValueIfHeaderAbsence) : this(headerKey, addValueIfHeaderAbsence, new HttpContextAccessor())
    {
    }

    internal CorelationIdEnricher(string headerKey, bool addValueIfHeaderAbsence, IHttpContextAccessor contextAccessor)
    {
        _headerKey = headerKey;
        _addValueIfHeaderAbsence = addValueIfHeaderAbsence;
        _contextAccessor = contextAccessor;
    }


    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _contextAccessor.HttpContext;
        if (httpContext == null)
        {
            return;
        }

        if (httpContext.Items[CorrelationIdItemKey] is LogEventProperty logEventProperty)
        {
            logEvent.AddPropertyIfAbsent(logEventProperty);
            return;
        }

        var header = httpContext.Request.Headers[_headerKey].ToString();
        var correlationId = !string.IsNullOrWhiteSpace(header)
            ? header
            : (_addValueIfHeaderAbsence ? Guid.NewGuid().ToString() : null);

        var correlationIdProperty = new LogEventProperty(PropertyName, new ScalarValue(correlationId));
        logEvent.AddOrUpdateProperty(correlationIdProperty);

        httpContext.Items.Add(CorrelationIdItemKey, correlationIdProperty);
    }
}
