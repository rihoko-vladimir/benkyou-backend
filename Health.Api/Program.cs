using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Logging.ClearProviders();
var loggerBuilder = new LoggerConfiguration()
    .WriteTo.Console();
if (builder.Environment.IsDevelopment())
    loggerBuilder.MinimumLevel.Debug();
else
    loggerBuilder.MinimumLevel.Information();
var logger = loggerBuilder.CreateLogger();
builder.Logging.AddSerilog(logger);

builder.Services.AddHealthChecksUI()
    .AddInMemoryStorage();

var app = builder.Build();

app.UseRouting();

//app.UseHttpsRedirection();

app.UseHealthChecksUI(options => options.UIPath = "/health-ui");

app.Run();