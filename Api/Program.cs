using Core.Data;
using Core.Extensions;
using Core.Utils;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using SpotifyDataExtractor.Extensions;


const string ServiceName = "service-api-v2";

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

#if DEBUG
builder.Logging.AddDebug();
#endif

var secrets = VaultHelper.GetSecrets(builder.Configuration, ServiceName);

var consulAddress = (string) secrets["consul-address"];
var consulToken = (string) secrets["consul-token"];
var storageName = ConsulHelper.BuildServiceName(builder.Configuration, ServiceName);

builder.Configuration.AddConsulConfiguration(consulAddress, consulToken, storageName);

builder.Logging.AddSentry(o =>
{
    o.Dsn = builder.Configuration["SentryDsn"];
    o.AttachStacktrace = true;
});

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

builder.Services.AddApiVersioning();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

builder.Services.AddMemoryCache();

builder.Services.AddLogging();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 0;
    logging.ResponseBodyLogLimit = 0;

});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsLocal())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
    //app.UseExceptionHandler();
}

app.UseHttpsRedirection();

//app.UseAuthorization();
app.UseSentryTracing();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new Asp.Versioning.ApiVersion(1, 0))
    .Build();

app.MapControllers()
    .WithApiVersionSet(versionSet);
app.MapHealthChecks("/health");

app.UseHttpLogging();

app.Run();