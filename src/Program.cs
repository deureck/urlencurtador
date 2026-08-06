using Microsoft.EntityFrameworkCore;
using urlencurtador.services;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Adiciona rastreamento de requisições HTTP para a dashboard
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("urlencurtador"))
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddOtlpExporter();
    });


builder.Services.AddControllers();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DBurl>(options=>options.UseNpgsql(connectionString));
builder.Services.AddScoped<ServicesUrl>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DBurl>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.Run();


