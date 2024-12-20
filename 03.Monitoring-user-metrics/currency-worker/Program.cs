using currency_worker.Analytics;
using currency_worker.Quartz;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<Ga4EventSender>();
builder.Services.AddSingleton<CurrencyJob>();

var host = builder.Build();

await host.Services.GetRequiredService<CurrencyJob>().ExecuteAsync();
