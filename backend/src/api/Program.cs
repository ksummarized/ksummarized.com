using api.Data;
using api.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using api;
using System.Security.Cryptography;

const string logFormat =  "[{Timestamp:HH:mm:ss} {Level:u3}] {CorelationId} | {Message:lj}{NewLine}{Exception}";
Log.Logger = new LoggerConfiguration().Enrich.WithCorrelationId()
                                             .WriteTo
                                             .Console(outputTemplate: logFormat)
                                             .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddHttpContextAccessor();
    builder.Host.UseSerilog();
    builder.Services.AddDbContext<UsersDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Users")));
    var keycloakJwtOptions = new KeycloakJwtOptions();
    builder.Configuration.Bind("KeycloakJwt", keycloakJwtOptions);
    if(keycloakJwtOptions.Secret is null || keycloakJwtOptions.Issuer is null || keycloakJwtOptions.Audience is null)
    {
        throw new Exception("Can't start application without JWT options");
    }

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
    builder.Services.AddSingleton(tokenValidationParameters);
    builder.Services.AddSingleton(keycloakJwtOptions);

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

    builder.Services.AddControllers();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "api", Version = "v1" });
    });
    builder.Services.AddScoped<IUserService, UserService>();

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
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "api v1"));
    }

    app.UseCors("AllowAll");
    app.UseAuthentication();

    app.UseHttpsRedirection();
    app.UseHsts();

    app.UseSerilogRequestLogging();
    app.UseRouting();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal("Error starting the application: {Exception}", ex);
}
finally
{
    Log.CloseAndFlush();
}
