using Api.Utils.HelthChecks;
using Core.Data;
using Core.Extensions;
using Core.Utils;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using SpotifyDataExtractor.Extensions;


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

builder.Services.AddCoreServices();

builder.Services.AddApiVersioning();
builder.Services.AddControllers()
    .AddControllersAsServices();
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

builder.Services.AddHealthChecks()
    .AddDbContextCheck<Context>()
    .AddCheck<ControllerResolveHealthCheck>(nameof(ControllerResolveHealthCheck));

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

//app.UseHttpLogging();
//app.UseSentryTracing();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new Asp.Versioning.ApiVersion(1, 0))
    .Build();

app.MapControllers()
    .WithApiVersionSet(versionSet);
app.MapHealthChecks("/health");

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
        options.UseNpgsql(connectionString);
    });

    builder.Services.AddPooledDbContextFactory<Context>(options =>
    {
        options.LogTo(Console.WriteLine);
        options.EnableSensitiveDataLogging(false);
        options.UseNpgsql(connectionString, optionsBuilder =>
        {
            optionsBuilder.EnableRetryOnFailure();
        });
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
    }, 16);
}