using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SafeCity.IRCM.HttpClients;
using SafeCity.IRCM.Repositories;
using SafeCity.IRCM.Services;
using SafeCity_IRCMDB.Data;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<SafeCityDbContext>(options =>
   options.UseSqlServer(b => b.MigrationsAssembly("SafeCity.IRCM")));
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<IIncidentRetrivalRepository, IncidentRetrivalRepository>();
builder.Services.AddScoped<IIncidentRetrivalService, IncidentRetrivalService>();
builder.Services.AddScoped<ICaseCreateRepository, CaseCreateRepository>();
builder.Services.AddScoped<ICaseCreateService, CaseCreateService>();
builder.Services.AddScoped<ICaseClosingService, CaseClosingService>();
builder.Services.AddScoped<ICaseClosingRepository, CaseClosingRepository>();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IIdentityService, IdentityService>(
    client => client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:IdentityService"]!)
    );
builder.Services.AddOpenApi();
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

// Middlewares 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
