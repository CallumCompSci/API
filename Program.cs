
using RepoDb;
using Npgsql;
using Interfaces;
using Last_Api.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();

builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();


var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string"
        + "'DefaultConnection' not found.");

builder.Services.AddScoped<IArtifact, ArtifactRepo>();
builder.Services.AddScoped<IArtEra, ArtEraRepo>();
builder.Services.AddScoped<ITribe, TribeRepo>();
builder.Services.AddScoped<IUser, UserRepo>();

GlobalConfiguration.Setup().UsePostgreSql();
builder.WebHost.UseUrls("https://localhost:5084", "http://localhost:5083");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "CallumsAPI",
        ValidAudience = "APIUser",
        RoleClaimType = ClaimTypes.Role,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("4b068b45b1614236b6052beee7e689c14f2180c122ff41d3a17778d49ef8283e"))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.RequireRole("admin"));
    options.AddPolicy("Staff", policy => policy.RequireRole("Staff", "admin"));
    options.AddPolicy("User", policy => policy.RequireRole("Staff", "admin", "User"));
});






var app = builder.Build();


app.MapOpenApi();


app.UseReDoc(options =>
{
    options.DocumentTitle = "Last API Docs";
    options.SpecUrl = "/swagger/v1/swagger.json"; 
    options.RoutePrefix = "docs";
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();


app.Run();

