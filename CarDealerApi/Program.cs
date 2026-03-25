using FastEndpoints;
using FastEndpoints.Swagger;
using FastEndpoints.Security;
using Data;

var builder = WebApplication.CreateBuilder(args);

// JWT
builder.Services.AddAuthenticationJwtBearer(s =>
{
    s.SigningKey = builder.Configuration["Jwt:Key"]!;
});
builder.Services.AddAuthorization();

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.ExcludeNonFastEndpoints = true;
    o.DocumentSettings = s => s.EnableJWTBearerAuth();
});


builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddSingleton<Data.DbInitializer>(); // init and seed DB

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
app.UseSwaggerGen();

// running DB initializer to create tables and seed data
using  (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<Data.DbInitializer>();
    await dbInitializer.InitializeAsync();
}

app.Run();
