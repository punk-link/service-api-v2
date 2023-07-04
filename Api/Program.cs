using Core.Extensions;
using Core.Utils;
using SpotifyDataExtractor.Extensions;


const string ServiceName = "service-api";

var builder = WebApplication.CreateBuilder(args);

var secrets = VaultHelper.GetSecrets(builder.Configuration, ServiceName);

var consulAddress = (string) secrets["consul-address"];
var consulToken = (string) secrets["consul-token"];
var storageName = ConsulHelper.BuildServiceName(builder.Configuration, ServiceName);

builder.Configuration.AddConsulConfiguration(consulAddress, consulToken, storageName);

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