using System.Text;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Experimental;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDocument();
builder.Services.AddEndpointsApiExplorer();

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

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseOpenApi();
app.UseFastEndpoints();
app.UseSwaggerGen();

app.Run();
