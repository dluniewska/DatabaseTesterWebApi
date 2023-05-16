using DatabaseTesterWebAPI.Services;
using Microsoft.EntityFrameworkCore;
using Tester.Data;
using Serilog;
using Serilog.Events;

string basedir = AppDomain.CurrentDomain.BaseDirectory;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(basedir + "/log/custom.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    Log.Logger.Information("Application started");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .MinimumLevel.Verbose()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(basedir + "/log/custom.log", rollingInterval: RollingInterval.Day);

    });

    builder.Services.AddControllers();
    builder.Services.AddDbContext<TesterContext>(
        o => o.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseTesterDb"))
    );
    builder.Services.AddDbContextFactory<TesterContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseTesterDb"));
    }, 
        ServiceLifetime.Scoped
    );

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddScoped<IBasicDbService, BasicInsertsService>();
    builder.Services.AddScoped<IBatchedInsertsService, BatchedInsertsService>();
    builder.Services.AddScoped<IHttpClientInsertsService, HttpClientInsertsService>();
    builder.Services.AddHttpClient();

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
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
