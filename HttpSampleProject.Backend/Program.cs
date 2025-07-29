using HttpSampleProject.Shared.ApplicationLayer.Requests;
using RequestResponseFramework.Server;
using RequestResponseFramework.Server.MiddlewareExecutors;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5222");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRequestResponseFramework(cfg =>
{
    cfg.RegisterHandlersFromAssemblyContaining<Program>();
    cfg.RegisterContractsFromAssemblyContaining<ListWeatherForecast>();
    cfg.AddMiddlewareExecutor<HandleSystemExceptionMiddlewareExecutor>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
