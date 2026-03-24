using System.Text;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o => o.ExcludeNonFastEndpoints = true);

// JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", Options =>
    {
        Options.TokenValidationParameters = new ()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("super secret key")
        )
    };
});
builder.Services.AddAuthorization();

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
