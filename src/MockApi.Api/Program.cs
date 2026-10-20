using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MockApi.Core.Configuration;
using MockApi.Core.Errors;
using MockApi.Core.Serialization;
using MockApi.Data.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(
    (context, options) =>
    {
        var maxRequestBodyBytes = context.Configuration.GetValue<long?>(
            $"{MockApiOptions.SectionName}:{nameof(MockApiOptions.Limits)}:{nameof(LimitsOptions.MaxRequestBodyBytes)}"
        );

        if (maxRequestBodyBytes is > 0)
        {
            options.Limits.MaxRequestBodySize = maxRequestBodyBytes;
        }
    }
);

builder.Services.AddMockApiDatabaseOptions(builder.Configuration);
builder.Services.AddMockApiOptions(builder.Configuration);
builder.Services.AddMockApiErrors(builder.Configuration);

builder
    .Services.AddControllers()
    .AddJsonOptions(options => JsonDefaults.Configure(options.JsonSerializerOptions));

builder.Services.ConfigureHttpJsonOptions(options =>
    JsonDefaults.Configure(options.SerializerOptions)
);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMockApiRequestContext();
app.UseMockApiErrorHandling();
app.UseMockApiManagementTransport();

app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });

app.MapControllers();

app.Run();

public partial class Program;
