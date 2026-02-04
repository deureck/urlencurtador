using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var connectionString = builder.Configuration.GetConnectionString("Postgress");
builder.Services.AddDbContext<DBurl>(options=>options.UseNpgsql(connectionString));
builder.Services.AddScoped<ServicesUrl>();
builder.Services.AddScoped<Base62Converter>();

var app = builder.Build();

        

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.Run();


