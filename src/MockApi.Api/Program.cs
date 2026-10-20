using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MockApi.Core.Configuration;
using MockApi.Core.Serialization;
using MockApi.Data.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMockApiDatabaseOptions(builder.Configuration);
builder.Services.AddMockApiData(builder.Configuration);
builder.Services.AddMockApiOptions(builder.Configuration);

builder
    .Services.AddControllers()
    .AddJsonOptions(options => JsonDefaults.Configure(options.JsonSerializerOptions));

builder.Services.ConfigureHttpJsonOptions(options =>
    JsonDefaults.Configure(options.SerializerOptions)
);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });

app.MapControllers();

app.Run();

public partial class Program;
