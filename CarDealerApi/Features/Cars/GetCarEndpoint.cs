using Dapper;
using FastEndpoints;
using Data;

namespace Features.Cars;

public class GetCarEndpoint : EndpointWithoutRequest<CarResponse>
{
    private readonly DbConnectionFactory _factory;

    public GetCarEndpoint(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public override void Configure()
    {
        Get("/cars/{id:int}");
        AllowAnonymous(); // TODO: add JWT
        Description(x => x.WithName("GetCar"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        using var connection = _factory.CreateConnection();
        var sql = "SELECT * FROM Cars WHERE Id = @Id";
        
        var car = await connection.QueryFirstOrDefaultAsync<CarResponse>(sql, new { Id = id });

        if (car == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        await Send.OkAsync(car, ct);
    }
}