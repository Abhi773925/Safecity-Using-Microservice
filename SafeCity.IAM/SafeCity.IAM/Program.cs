using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SafeCity.IAM.Repositories;
using SafeCity.IAM.Services;
using SafeCity.IAMDB.Data;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

//  Authentication Services
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

//  Authorization Services
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
var connectionString = builder.Configuration.GetConnectionString("IAMDatabase");

if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = "Server=LTIN718874\\SQLEXPRESS;Database=SafeCity_IAMDB;Trusted_Connection=True;TrustServerCertificate=True;";
}

builder.Services.AddDbContext<SafeCityDbContext>(options =>
{
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("SafeCity.IAM"));
});
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGateway", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowGateway");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();