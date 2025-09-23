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

// CORS পলিসি সংজ্ঞায়িত করো
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:4200") // ফ্রন্টএন্ড URL (HTTP, HTTPS নয়)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // SignalR-এর জন্য ক্রেডেনশিয়াল সাপোর্ট
    });
});

// JWT Authentication
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

// Configure the HTTP request pipeline.
app.UseCors("AllowSpecificOrigin"); // নামযুক্ত CORS পলিসি ব্যবহার করো

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