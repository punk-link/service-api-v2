using Core.Data;
using Core.Extensions;
using Core.Utils;
using Microsoft.EntityFrameworkCore;
using PresentationGrpc.Services;
using System.Net;


const string ServiceName = "service-api";

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddCoreServices();

builder.Services.AddScoped<IArtistPresentationService, ArtistPresentationService>();

builder.Services.AddGrpc();
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Loopback, 15170, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

//builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapGrpcService<PresentationService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
