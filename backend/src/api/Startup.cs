using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace api
{
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "api", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowAll", builder =>
                     {
                         builder.WithOrigins("*");
                     });
            });

            var postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
            var postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            var postgresDb = Environment.GetEnvironmentVariable("POSTGRES_DB");
            var postgresHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
            var postgresPort = Environment.GetEnvironmentVariable("POSTGRES_PORT");
            var connectionString = $"host={postgresHost};port={postgresPort};database={postgresDb};username={postgresUser};password={postgresPassword}";

            services.AddDbContext<ApiDbContext>(options => 
                options.UseNpgsql(connectionString)
            );
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
}
