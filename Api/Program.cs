using Core.Data;
using Core.Extensions;
using Core.Utils;
using Microsoft.EntityFrameworkCore;
using SpotifyDataExtractor.Extensions;


const string ServiceName = "service-api";

var builder = WebApplication.CreateBuilder(args);

var root = (IConfigurationRoot)  builder.Configuration;
Console.WriteLine(root.GetDebugView());

var secrets = VaultHelper.GetSecrets(builder.Configuration, ServiceName);

var consulAddress = (string) secrets["consul-address"];
var consulToken = (string) secrets["consul-token"];
var storageName = ConsulHelper.BuildServiceName(builder.Configuration, ServiceName);

builder.Configuration.AddConsulConfiguration(consulAddress, consulToken, storageName);

var password = (string) secrets["database-password"];
var userId = (string) secrets["database-username"];
var connectionString = DatabaseHelper.BuildConnectionString(builder.Configuration, userId, password);

builder.Services.AddDbContextPool<Context>(options =>
{
    options.LogTo(Console.WriteLine);
    options.EnableSensitiveDataLogging(false);
    options.UseNpgsql(connectionString, optionsBuilder =>
    {
        optionsBuilder.EnableRetryOnFailure();
    });
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
}, 16);

builder.Services.AddSpotifyDataExtractor(options =>
{
    options.ClientId = builder.Configuration["SpotifySettings:ClientId"]!;
    options.ClientSecret = secrets["spotify-client-secret"];
});

// TODO: https://learn.microsoft.com/en-us/ef/core/performance/efficient-updating?tabs=ef7

builder.Services.AddApiVersioning();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

builder.Services.AddMemoryCache();

builder.Services.AddLogging();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsLocal())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new Asp.Versioning.ApiVersion(1, 0))
    .Build();

app.MapControllers()
    .WithApiVersionSet(versionSet);
app.MapHealthChecks("/health");

app.Run();