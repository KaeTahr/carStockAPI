using Dapper;
using FastEndpoints;
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
        AllowAnonymous(); // TODO: add JWT
        Description(x => x.WithName("DeleteCar"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        using var connection = _factory.CreateConnection();

        var sql = "DELETE FROM Cars WHERE Id = @Id";
        
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });

        if (affectedRows == 0)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}