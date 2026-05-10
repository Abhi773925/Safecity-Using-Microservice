using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SafeCity.PFOM.HttpClients;
using SafeCity.PFOM.Repositories;
using SafeCity.PFOM.Services;
using SafeCity_PFOMDB.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IPatrolRepository, PatrolRepository>();
builder.Services.AddScoped<IPatrolService, PatrolService>();
builder.Services.AddScoped<IFieldReportRepository, FieldReportRepository>();
builder.Services.AddScoped<IFieldReportService, FieldReportService>();
builder.Services.AddDbContext<SafeCityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PFOMDatabase"), b =>
        b.MigrationsAssembly("SafeCity_PFOMDB")));
builder.Services.AddHttpClient<IIdentityService, IdentityService>(
    client => client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:IdentityService"]!)
    );
builder.Services.AddSwaggerGen();

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGateway", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowGateway");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
