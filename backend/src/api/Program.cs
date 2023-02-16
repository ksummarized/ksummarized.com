using api.Data.DAO;
using api.Data;
using api.Services.Tokens;
using api.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UsersDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Users")));
var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:SECRET"])),
    ValidateAudience = true,
    ValidAudience = builder.Configuration["JWT:AUDIENCE"],
    ValidateIssuer = true,
    ValidIssuer = builder.Configuration["JWT:ISSUER"],
    ValidateLifetime = true
};
builder.Services.AddSingleton(tokenValidationParameters);
builder.Services.AddIdentity<UserModel, IdentityRole>().AddEntityFrameworkStores<UsersDbContext>().AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = builder.Environment.IsProduction();
    options.TokenValidationParameters = tokenValidationParameters;

});
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "api", Version = "v1" });
});
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll", builder =>
    {
        builder.WithOrigins("*");
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
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

app.MapControllers();

app.Run();