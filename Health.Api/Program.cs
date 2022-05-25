var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddHealthChecksUI()
    .AddInMemoryStorage();

var app = builder.Build();

app.UseRouting();

//app.UseHttpsRedirection();
app.UseEndpoints(routeBuilder => { routeBuilder.MapHealthChecksUI(options => options.UIPath = "/health-ui"); });

app.Run();