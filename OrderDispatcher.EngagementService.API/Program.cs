using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderDispatcher.EngagementService.API.Configuration;
using OrderDispatcher.EngagementService.API.Entities;
using OrderDispatcher.EngagementService.API.Services;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<EngagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtTokenOptions"));

var jwtSection = builder.Configuration.GetSection("JwtTokenOptions");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<StoreService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EngagementDbContext>();
    db.Database.Migrate();
}

app.MapHealthChecks("/health");

app.MapOpenApi();
app.MapScalarApiReference("/docs", o => o.Title = "Engagement Service API");
app.MapGet("/", () => Results.Redirect("/docs"));

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
