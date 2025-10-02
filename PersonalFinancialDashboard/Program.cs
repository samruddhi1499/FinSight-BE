using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalFinancialDashboard.Entities;
using PersonalFinancialDashboard.Services;
using PersonalFinancialDashboard.Services.Interface;
using Scalar.AspNetCore;
using System.Text;




var builder = WebApplication.CreateBuilder(args);

// CORS Policy to allow frontend calls with credentials
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")  // Exact frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// JWT Authentication reading token from the cookie
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["jwt_token"];
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "Authentication failed");
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]))
        };
    });


//var dbServer = Environment.GetEnvironmentVariable("DB_SERVER") ;
//var dbName = Environment.GetEnvironmentVariable("DB_NAME") ;
//var dbUser = Environment.GetEnvironmentVariable("DB_USER");
//var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

//var connectionString = $"Server={dbServer};Port=3306;Database={dbName};User={dbUser};Password={dbPassword};SslMode=None;";

var dbServer = "bobpnzddfztu2tm8mrkt-mysql.services.clever-cloud.com";
var dbName = "bobpnzddfztu2tm8mrkt";
var dbUser = "uhpvcvted55itidb";
var dbPassword = "Fuogw9qsZcyaBOXJfffm"; // replace with actual password



var connectionString = $"Server={dbServer};Port=3306;Database={dbName};User={dbUser};Password={dbPassword};SslMode=None;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));



// Add services to the container.

builder.Services.AddControllers();
// In Program.cs
builder.Services.AddScoped<IExpenseService, ExpenseServiceImpl>();
builder.Services.AddScoped<IAuthService, AuthServiceImpl>();
builder.Services.AddScoped<IUserDetailsService, UserDetailsServiceImpl>();
builder.Services.AddScoped<IRecurringCategoriesService, RecurringCategoriesServiceImpl>();
builder.Services.AddScoped<IDasboardService, DashboardServiceImpl>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost3000");


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
