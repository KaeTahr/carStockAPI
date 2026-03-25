using Dapper;
using FastEndpoints;
using Data;

namespace Features.Auth;

public class RegisterEndpoint : Endpoint<RegisterRequest>
{
    private readonly DbConnectionFactory _factory;

    public RegisterEndpoint(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
        Description(x => x.WithName("Register"));
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        using var connection = _factory.CreateConnection();

        // check if username already exists
        var existing = await connection.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM Dealers WHERE Username = @Username",
            new { req.Username });

        if (existing > 0)
        {
            AddError(r=> r.Username, "Username already exists");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);

        await connection.ExecuteAsync(@"
            INSERT INTO Dealers (Name, Username, PasswordHash)
            VALUES (@Name, @Username, @PasswordHash)",
            new { req.Name, req.Username, PasswordHash = passwordHash });

        await Send.OkAsync(new { Message = "Dealer registered successfully!" }, ct);
    }
}