using Api.Utils.HelthChecks;
using Core.Data;
using Core.Extensions;
using Core.Utils;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifyDataExtractor.Extensions;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();

//#if DEBUG
//builder.Logging.AddDebug();
//#endif

var secrets = VaultHelper.GetSecrets(builder.Configuration);
AddConsulConfiguration(builder, secrets);

//builder.Logging.AddSentry(o =>
//{
//    o.Dsn = builder.Configuration["SentryDsn"];
//    o.AttachStacktrace = true;
//});

AddContexts(builder, secrets);

builder.Services.AddSpotifyDataExtractor(options =>
{
    options.ClientId = builder.Configuration["SpotifySettings:ClientId"]!;
    options.ClientSecret = secrets["spotify-client-secret"]!;
});

builder.Services.AddApiVersioning();
builder.Services.AddMemoryCache();

builder.Services.AddCoreServices();
builder.Services.AddControllers()
    .AddControllersAsServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 0;
    logging.ResponseBodyLogLimit = 0;

});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<Context>()
    .AddCheck<ControllerResolveHealthCheck>(nameof(ControllerResolveHealthCheck));

var app = builder.Build();

app.UseExceptionHandler(ConfigureExceptionHandler(app));

if (app.Environment.IsDevelopment() || app.Environment.IsLocal())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHsts();
app.UseHttpsRedirection();

app.UseHealthChecks("/health");

//app.UseHttpLogging();
//app.UseSentryTracing();

app.UseAuthentication();
app.UseAuthorization();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new Asp.Versioning.ApiVersion(1, 0))
    .Build();

app.MapControllers()
    .WithApiVersionSet(versionSet);

app.Run();


static void AddConsulConfiguration(WebApplicationBuilder builder, dynamic secrets)
{
    var consulAddress = (string)secrets["consul-address"]!;
    var consulToken = (string)secrets["consul-token"]!;
    var storageName = ConsulHelper.BuildServiceName(builder.Configuration);

    builder.Configuration.AddConsulConfiguration(consulAddress, consulToken, storageName);
}


static void AddContexts(WebApplicationBuilder builder, dynamic secrets)
{
    var password = (string)secrets["database-password"]!;
    var userId = (string)secrets["database-username"]!;
    var connectionString = DatabaseHelper.BuildConnectionString(builder.Configuration, userId, password);

    builder.Services.AddDbContext<Context>(options =>
    {
        options.LogTo(Console.WriteLine);
        options.EnableSensitiveDataLogging(false);
        options.UseNpgsql(connectionString, optionsBuilder =>
        {
            optionsBuilder.EnableRetryOnFailure();
        });
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
    }, optionsLifetime: ServiceLifetime.Singleton);

    builder.Services.AddPooledDbContextFactory<Context>(options =>
    {
        options.LogTo(Console.WriteLine);
        options.EnableSensitiveDataLogging(false);
        options.UseNpgsql(connectionString, optionsBuilder =>
        {
            optionsBuilder.EnableRetryOnFailure();
        });
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
    });
}


static Action<IApplicationBuilder> ConfigureExceptionHandler(WebApplication app) 
    => handler => handler.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error is not null)
        {
            var details = new ProblemDetails
            {
                Detail = exceptionHandlerPathFeature?.Error.Message,
                Instance = exceptionHandlerPathFeature!.Endpoint?.ToString(),
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            };

            details.Extensions.Add("trace-id", Activity.Current?.Id);

            if (app.Environment.IsDevelopment() || app.Environment.IsLocal())
                details.Extensions.Add("stack-trace", exceptionHandlerPathFeature!.Error!.StackTrace?.ToString());

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(details);
        }
    });