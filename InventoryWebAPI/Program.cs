using InventoryWebAPI.Application.Interfaces;
using InventoryWebAPI.Application.Interfaces.Inventory;
using InventoryWebAPI.Domain.Entities;
using InventoryWebAPI.Hubs;
using InventoryWebAPI.Infrastructure.Data;
using InventoryWebAPI.Infrastructure.Repositories;
using InventoryWebAPI.Infrastructure.Repositories.Inventory;
using InventoryWebAPI.Infrastructure.Security;
using InventoryWebAPI.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adding PostgreSQL ConnectionString
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Repositories & UnitOfWork
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// JWTService
builder.Services.AddScoped<IJwtService, JWTService>();

// IdentityCore Service
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = false;
})
    .AddRoles<IdentityRole>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager<SignInManager<User>>()
    .AddUserManager<UserManager<User>>()
    .AddDefaultTokenProviders();

// SignalR
builder.Services.AddSignalR();

// CORS Policy - appsettings.json থেকে ডায়নামিকভাবে নেওয়া
var clientUrls = builder.Configuration.GetSection("JWT:ClientUrl").Get<string>();
var allowedOrigins = new List<string>();

if (!string.IsNullOrEmpty(clientUrls))
{
    // Multiple URLs সাপোর্ট করার জন্য (comma separated)
    var urls = clientUrls.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(url => url.Trim())
                         .Where(url => !string.IsNullOrEmpty(url))
                         .ToArray();

    allowedOrigins.AddRange(urls);
}

// Fallback for development
if (allowedOrigins.Count == 0)
{
    allowedOrigins.Add("http://localhost:4200");
    allowedOrigins.Add("https://localhost:4200");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", corsBuilder =>
    {
        corsBuilder.WithOrigins(allowedOrigins.ToArray())
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .SetIsOriginAllowed((host) => true);
    });
});

// JWT Authentication - SignalR এর জন্য বিশেষ কনফিগারেশন
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // SignalR এর জন্য JWT events কনফিগার করুন
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/progressHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Modifying Server-Side Error Messages
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage).ToArray();

        var toReturn = new
        {
            Errors = errors
        };

        return new BadRequestObjectResult(toReturn);
    };
});

var app = builder.Build();


app.UseCors("AllowSpecificOrigin");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ProgressHub>("/progressHub");
app.Run();