using Dapper;
using FastEndpoints;
using Data;

namespace Features.Cars;

public class GetAllCarsEndpoint : EndpointWithoutRequest<List<CarResponse>>
{
    private readonly DbConnectionFactory _factory;

    public GetAllCarsEndpoint(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public override void Configure()
    {
        Get("/cars");
        AllowAnonymous(); // TODO: add JWT
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var connection = _factory.CreateConnection();
        var sql = "SELECT * FROM Cars";
        var cars = await connection.QueryAsync<CarResponse>(sql);
        await Send.OkAsync(cars.ToList(), ct);
    }
}