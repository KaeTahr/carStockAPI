using Dapper;
using FastEndpoints;
using Data;
using Extensions;

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
        var dealerId = HttpContext.GetDealerId();
        using var connection = _factory.CreateConnection();
        var sql = "SELECT * FROM Cars WHERE DealerId = @DealerId";
        var cars = await connection.QueryAsync<CarResponse>(sql, new { DealerId = dealerId });
        await Send.OkAsync(cars.ToList(), ct);
    }
}