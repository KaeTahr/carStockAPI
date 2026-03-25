using Dapper;
using FastEndpoints;
using Data;
using Extensions;

namespace Features.Cars;

public class SearchCarsRequest
{
    [QueryParam] public string? Make { get; set; }
    [QueryParam] public string? Model { get; set; }
}

public class SearchCarsEndpoint : Endpoint<SearchCarsRequest, List<CarResponse>>
{
    private readonly DbConnectionFactory _factory;

    public SearchCarsEndpoint(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public override void Configure()
    {
        Get("/cars/search");
        Claims("dealerId");
        Description(x => x.WithName("SearchCars"));
    }

    public override async Task HandleAsync(SearchCarsRequest req, CancellationToken ct)
    {
        using var connection = _factory.CreateConnection();
        var dealerId = HttpContext.GetDealerId();

        var sql = @"
            SELECT * FROM Cars
            WHERE DealerId = @DealerId
            AND (@Make IS NULL OR Make LIKE @Make)
            AND (@Model IS NULL OR Model LIKE @Model);";

        var cars = await connection.QueryAsync<CarResponse>(sql, new
        {
            DealerId = dealerId,
            Make = req.Make == null ? null : $"%{req.Make}%",
            Model = req.Model == null ? null : $"%{req.Model}%"
        });

        await Send.OkAsync(cars.ToList(), ct);
    }
}