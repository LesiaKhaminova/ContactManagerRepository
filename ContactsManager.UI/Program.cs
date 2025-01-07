using Serilog;
using MyFirstApplication.SturtupExtensions;
using MyFirstApplication.Middleware;

var builder = WebApplication.CreateBuilder(args);
//serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services);
});
builder.Services.ConfigureServices(builder.Configuration);
    //logging
/*builder.Host.ConfigureLogging(loggingProvider =>
{
    loggingProvider.ClearProviders();
    loggingProvider.AddConsole();
    loggingProvider.AddDebug();
    loggingProvider.AddEventLog();
});*/


var app = builder.Build();
app.UseSerilogRequestLogging();
if(builder.Environment.IsEnvironment("Test") == false)
  Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot",wkhtmltopdfRelativePath: "Rotativa" );
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}
app.UseHsts();
app.UseHttpsRedirection();
app.UseHttpLogging();
//app.Logger.LogDebug("debug-message");
//app.Logger.LogInformation("information-message");
//app.Logger.LogWarning("warning-message");
//app.Logger.LogError("error-message");
//app.Logger.LogCritical("critical-message");

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseEndpoints(endpoint =>
{
     endpoint.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}");
});
app.Run();

public partial class Program { }