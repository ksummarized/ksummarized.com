using api.Data;
using api.Data.DAO;
using api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {

        services.AddDbContext<UsersDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("Users")));
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JWT:secret"])),
            ValidateAudience = true,
            ValidAudience = Configuration["JWT:audience"],
            ValidateIssuer = true,
            ValidIssuer = Configuration["JWT:issuer"],
            ValidateLifetime = true,
            ClockSkew = System.TimeSpan.Zero
        };
        services.AddSingleton(tokenValidationParameters);
        services.AddIdentity<UserModel, IdentityRole>().AddEntityFrameworkStores<UsersDbContext>().AddDefaultTokenProviders();
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireDigit = true;
            options.Lockout.MaxFailedAccessAttempts = 3;
        });
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = tokenValidationParameters;
        });
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "api", Version = "v1" });
        });
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddCors(options =>
        {
            options.AddPolicy(name: "AllowAll", builder =>
                 {
                     builder.WithOrigins("*");
                 });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "api v1"));
        }

        app.UseAuthentication();

        app.UseCors("AllowAll");

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseAuthorization();


        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
