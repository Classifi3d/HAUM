using HAUM_BackEnd.Context;
using HAUM_BackEnd.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.Filters;
using System.Net.WebSockets;
using HAUM_BackEnd.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// Swagger Endpoints

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder
    .Services
    .AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition(
            "oauth2",
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description =
                    "Standard Authorization Header Using The Bearer Scheme (\"bearer {token}\")",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
            }
        );
        ;
        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });


// JWT Token
builder
    .Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)
            ),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Cross-Origin Resource Sharing
var allowSpecificOrigin = "_myAllowSpecificOrigins";

builder
    .Services
    .AddCors(options =>
    {
        options.AddPolicy(
            allowSpecificOrigin,
            builder =>
            {
                builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            }
        );
    });


// PostGresDB
builder
    .Services
    .AddDbContext<ApplicationDbContext>(
        options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("HAUM_PostgreSQL_Connection_String")
            )
    );

// Depedency Injections
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IDataRepository, DataRepository>();
builder.Services.AddSingleton( 
    wsm => {
        var scopeFactory = wsm.GetRequiredService<IServiceScopeFactory>();
        return new HAUM_BackEnd.WebSockets.WebSocketManager(scopeFactory, retryIntervalSeconds: 30);
    });
builder.Services.AddHostedService<WebSocketBackgroundService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(allowSpecificOrigin);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();