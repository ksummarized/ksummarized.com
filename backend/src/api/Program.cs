using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Cryptography;
using infrastructure.Data;
using infrastructure.Keycloak;
using infrastructure.Logging;
using api.Authorization;
using api.Middleware;
using Microsoft.AspNetCore.Authorization;
using api.Endpoints;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog.Sinks.OpenTelemetry;

const string logFormat = "[{Timestamp:HH:mm:ss} {Level:u3}] {CorelationId} | {Message:lj}{NewLine}{Exception}";
var otelServiceName = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME")
    ?? Environment.GetEnvironmentVariable("OpenTelemetry__ServiceName")
    ?? "ksummarized.api";
var otelServiceVersion = typeof(Program).Assembly.GetName().Version?.ToString();
var otelEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")
    ?? Environment.GetEnvironmentVariable("OpenTelemetry__Otlp__Endpoint")
    ?? "http://localhost:18889";
var otelProtocol = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL")
    ?? Environment.GetEnvironmentVariable("OpenTelemetry__Otlp__Protocol")
    ?? "grpc";
var logConfig = new LoggerConfiguration().Enrich.WithCorrelationId()
                                             .Enrich.FromLogContext()
                                             .WriteTo
                                             .Console(outputTemplate: logFormat)
                                             .WriteTo
                                             .OpenTelemetry(options =>
                                             {
                                                 options.Endpoint = otelEndpoint;
                                                 if (string.Equals(otelProtocol, "http/protobuf", StringComparison.OrdinalIgnoreCase))
                                                 {
                                                     options.Protocol = OtlpProtocol.HttpProtobuf;
                                                 }
                                                 else if (string.Equals(otelProtocol, "grpc", StringComparison.OrdinalIgnoreCase))
                                                 {
                                                     options.Protocol = OtlpProtocol.Grpc;
                                                 }

                                                 options.IncludedData = IncludedData.TraceIdField
                                                     | IncludedData.SpanIdField
                                                     | IncludedData.MessageTemplateTextAttribute;
                                                 options.ResourceAttributes = new Dictionary<string, object>
                                                 {
                                                     ["service.name"] = otelServiceName,
                                                     ["service.version"] = otelServiceVersion ?? "unknown",
                                                     ["service.instance.id"] = Environment.MachineName
                                                 };
                                             });
Log.Logger = logConfig.CreateLogger();

try
{
    // Allow OTLP/gRPC over HTTP for local Aspire Dashboard usage.
    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddHttpContextAccessor();
    builder.Host.UseSerilog();
    ConfigureOpenTelemetry(builder);
    builder.Services.AddDbContext<ApplicationDbContext>(
        options => options.UseNpgsql(builder.Configuration.GetConnectionString("KSummarized"),
        x => x.MigrationsAssembly("infrastructure")
    ));
    var keycloakJwtOptions = builder.Configuration.GetRequiredSection("KeycloakJwt").Get<KeycloakJwtOptions>()!;

    // Create RSA key for offline validation of Keycloak token
    RSA rsa = RSA.Create();
    rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(keycloakJwtOptions.Secret), out _);
    var rsaKeycloakSecurityKey = new RsaSecurityKey(rsa)
    {
        KeyId = Guid.NewGuid().ToString()
    };

    var tokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = rsaKeycloakSecurityKey,
        ValidAudience = keycloakJwtOptions.Audience,
        ValidateAudience = true,
        ValidIssuer = keycloakJwtOptions.Issuer,
        ValidateIssuer = true,
        ValidateLifetime = true
    };

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = tokenValidationParameters;
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "api", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>() 
            }
        });
    });
    builder.Services.AddSingleton<IAuthorizationHandler, UserIdRequirementHandler>();
    builder.Services.AddAuthorizationBuilder()
        .AddPolicy(UserIdRequirement.PolicyName, p => p.AddRequirements(new UserIdRequirement()));

    builder.Services.AddTodoServices();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: "AllowAll", builder =>
        {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseExceptionHandlers();
    app.UseCors("AllowAll");

    app.UseHttpsRedirection();
    app.UseHsts();

    app.UseSerilogRequestLogging();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapEndpoints();
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error starting the application: {Exception}", ex);
}
finally
{
    await Log.CloseAndFlushAsync();
}

static void ConfigureOpenTelemetry(WebApplicationBuilder builder)
{
    var configuration = builder.Configuration;
    var serviceName = configuration["OTEL_SERVICE_NAME"]
        ?? configuration["OpenTelemetry:ServiceName"]
        ?? builder.Environment.ApplicationName;
    var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString();

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService(
            serviceName,
            serviceVersion: serviceVersion,
            serviceInstanceId: Environment.MachineName))
        .WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                .AddHttpClientInstrumentation(options => options.RecordException = true)
                .AddEntityFrameworkCoreInstrumentation()
                .AddOtlpExporter(options => ConfigureOtlpExporter(options, configuration));
        })
        .WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options => ConfigureOtlpExporter(options, configuration));
        });
}

static void ConfigureOtlpExporter(OtlpExporterOptions options, IConfiguration configuration)
{
    var endpoint = configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]
        ?? configuration["OpenTelemetry:Otlp:Endpoint"];
    if (!string.IsNullOrWhiteSpace(endpoint))
    {
        options.Endpoint = new Uri(endpoint);
    }

    var protocol = configuration["OTEL_EXPORTER_OTLP_PROTOCOL"]
        ?? configuration["OpenTelemetry:Otlp:Protocol"];
    if (string.Equals(protocol, "http/protobuf", StringComparison.OrdinalIgnoreCase))
    {
        options.Protocol = OtlpExportProtocol.HttpProtobuf;
    }
    else if (string.Equals(protocol, "grpc", StringComparison.OrdinalIgnoreCase))
    {
        options.Protocol = OtlpExportProtocol.Grpc;
    }
}
