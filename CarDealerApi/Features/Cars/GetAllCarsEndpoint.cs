using Dapper;
using FastEndpoints;
using FastEndpoints.Security;
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
        Claims("dealerId");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var dealerId = int.Parse(User.ClaimValue("dealerId")!);
        using var connection = _factory.CreateConnection();
        var sql = "SELECT * FROM Cars WHERE DealerId = @DealerId";
        var cars = await connection.QueryAsync<CarResponse>(sql, new { DealerId = dealerId });
        await Send.OkAsync(cars.ToList(), ct);
    }
}