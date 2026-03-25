using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using FastEndpoints;
using Microsoft.IdentityModel.Tokens;
using Data;

namespace Features.Auth;

public class DealerRecord
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly DbConnectionFactory _factory;
    private readonly IConfiguration _config;

    public LoginEndpoint(DbConnectionFactory factory, IConfiguration config)
    {
        _factory = factory;
        _config = config;
    }

    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
        Description(x => x.WithName("Login"));
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        using var connection = _factory.CreateConnection();

        var dealer = await connection.QueryFirstOrDefaultAsync<DealerRecord>(
            "SELECT Id, Username, PasswordHash FROM Dealers WHERE Username = @Username",
            new { req.Username });

        if (dealer == null || !BCrypt.Net.BCrypt.Verify(req.Password, dealer.PasswordHash))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: new[]
            {
                new Claim("dealerId", dealer.Id.ToString()),
                new Claim(ClaimTypes.Name, dealer.Username)
            },
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        await Send.OkAsync(new LoginResponse { Token = tokenString }, ct);
    }
}