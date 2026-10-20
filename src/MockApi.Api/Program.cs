using MockApi.Api.Observability;
using MockApi.Core.Configuration;
using MockApi.Core.Serialization;
using MockApi.Data.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddMockApiLogging();

builder.Services.AddMockApiDatabaseOptions(builder.Configuration);
builder.Services.AddMockApiOptions(builder.Configuration);

builder
    .Services.AddControllers()
    .AddJsonOptions(options => JsonDefaults.Configure(options.JsonSerializerOptions));

builder.Services.ConfigureHttpJsonOptions(options =>
    JsonDefaults.Configure(options.SerializerOptions)
);

builder.Services.AddMockApiHealthChecks();

var app = builder.Build();

app.UseMockApiRequestLogging();

app.MapMockApiHealthChecks();
app.MapControllers();

app.Run();

public partial class Program;
