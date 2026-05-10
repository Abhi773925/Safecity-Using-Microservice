using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Ocelot Config Load
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add Controllers (Swagger generator ko iski zaroorat hoti hai)
builder.Services.AddControllers();

// Add Swagger Gen (Ye missing tha, isliye error aa raha tha)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MMLib Swagger For Ocelot
builder.Services.AddSwaggerForOcelot(builder.Configuration);

// Standard Ocelot
builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Order ka dhyan rakhna bhai
app.UseRouting();
app.UseCors("AllowAll");

// Swagger For Ocelot UI (Dropdown wala magic)
app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

// Ocelot Middleware hamesha last mein
await app.UseOcelot();

app.Run();