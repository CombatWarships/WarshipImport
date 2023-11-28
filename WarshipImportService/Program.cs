using Azure.Core;
using Azure.Identity;
using Serilog;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using ServiceBus.Core;
using WarshipEnrichment;
using WarshipEnrichmentAPI;
using WarshipImport;
using WarshipImport.Databases;
using WarshipImport.Interfaces;
using WarshipImport.Managers;
using WarshipImport.Middleware;

Log.Logger = new LoggerConfiguration()
		 .WriteTo.Console()
		 .CreateBootstrapLogger();
Log.Information("Application Started");

var builder = WebApplication.CreateBuilder(args);
Log.Information("Builder created");


TokenCredential keyvaultCredential = new DefaultAzureCredential();
if (builder.Environment.IsDevelopment())
{
	keyvaultCredential = new ClientSecretCredential(builder.Configuration["tenantID"], builder.Configuration["clientID"], builder.Configuration["clientSecret"]);
}

var url = $"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/";
builder.Configuration.AddAzureKeyVault(new Uri(url), keyvaultCredential);



var appInsightsConnection = builder.Configuration["AppInsights"];
if (string.IsNullOrEmpty(appInsightsConnection))
	Log.Error("Application Insights Connection is NULL");

Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
	.WriteTo.ApplicationInsights(appInsightsConnection, new TraceTelemetryConverter())
	.WriteTo.Console()
	.CreateLogger();

Log.Information("Application started & Logger attached");

builder.Logging.AddSerilog(Log.Logger);

var connectionString = builder.Configuration["DBConnection"];
if (string.IsNullOrEmpty(connectionString))
	Log.Error($"Database connection string is NULL");

// Add services to the container.
builder.Services.AddSingleton<IWarshipProcessorAPI, WarshipProcessorAPI>((sp) =>
{
	return new WarshipProcessorAPI(sp.GetRequiredService<IConfiguration>()["WarshipEnrichment"]!);
});

builder.Services.AddScoped<IProposedShipsDatabase, ProposedShipDatabase>();
builder.Services.AddScoped<IShipList, IrcwccShipList>();
builder.Services.AddSingleton<IMessageProcessor, ConflictedShipProcessor>();

builder.Services.AddSingleton<IHostedService, WarshipConflictConsumerHost>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseCors(options =>
	options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var conflictProcessor = app.Services.GetService<IHostedService>();

var tokenSource = new CancellationTokenSource();
var t = conflictProcessor!.StartAsync(tokenSource.Token);

app.Run();

tokenSource.Cancel();
await t;
