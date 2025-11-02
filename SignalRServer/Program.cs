using Microsoft.AspNetCore.SignalR;
using SignalRServer.Hubs;
using SignalRServer.Models;
using SignalRServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<Facade>(sp =>
{
    var hubContext = sp.GetRequiredService<IHubContext<GameHub>>();
    return Facade.GetInstance(hubContext);
});

// Add SignalR
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
options.AddPolicy("CorsPolicy", builder => builder
    .WithOrigins("http://localhost:3000") // Note: https for React
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(_ => true) // Allow any origin for development
    .AllowCredentials()
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseAuthorization();

app.MapControllers();
app.MapHub<GameHub>("/gameHub");

app.Run("http://localhost:5000");