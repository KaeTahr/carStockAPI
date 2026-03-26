using Dapper;
using FastEndpoints;
using FastEndpoints.Security;
using Data;

namespace Features.Cars;

public class DeleteCarEndpoint : EndpointWithoutRequest
{
    private readonly DbConnectionFactory _factory;

    public DeleteCarEndpoint(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public override void Configure()
    {
        Delete("/cars/{id:int}");
        Claims("dealerId");
        Description(x => x.WithName("DeleteCar"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        var dealerId = int.Parse(User.ClaimValue("dealerId")!);
        using var connection = _factory.CreateConnection();

        var sql = "DELETE FROM Cars WHERE Id = @Id AND DealerId = @DealerId";

        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, DealerId = dealerId });

        if (affectedRows == 0)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}