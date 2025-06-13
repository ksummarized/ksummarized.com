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

// Top-level statements must come before type declarations.
// All using directives and actual code (top-level statements) go here.

const string logFormat = "[{Timestamp:HH:mm:ss} {Level:u3}] {CorelationId} | {Message:lj}{NewLine}{Exception}";
var logConfig = new LoggerConfiguration().Enrich.WithCorrelationId()
                                             .WriteTo
                                             .Console(outputTemplate: logFormat);
Log.Logger = logConfig.CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddHttpContextAccessor();
    builder.Host.UseSerilog();
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

// Make Program class public for WebApplicationFactory.
// This must be at the end of the file for top-level statements.
public partial class Program { }
