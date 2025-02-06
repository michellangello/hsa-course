using Beanstalk.Core;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddKeyedSingleton<IConnectionMultiplexer>("RedisRdb",
	ConnectionMultiplexer.Connect("localhost:6379"));

builder.Services.AddKeyedSingleton<IConnectionMultiplexer>("RedisAof",
	ConnectionMultiplexer.Connect("localhost:6380"));

builder.Services.AddScoped<BeanstalkConnection>(sp =>
	new BeanstalkConnection("localhost", 11300));

builder.Services.AddControllers(); // Registers controller classes automatically
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers(); // Automatically maps all controllers

app.Run();
