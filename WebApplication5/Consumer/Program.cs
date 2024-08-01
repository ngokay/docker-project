using Consumer.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<WorkerBackgroundService>();


builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });
builder.Services.AddSingleton<ConnectionRabbitMQHepperExtension>();
builder.Services.AddSingleton<ConnectionRabbitMQHepper>();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
