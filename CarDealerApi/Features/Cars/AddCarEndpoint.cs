using Dapper;
using FastEndpoints;
using FastEndpoints.Security;
using Data;


namespace Features.Cars;

public class AddCarEndpoint : Endpoint<AddCarRequest, CarResponse>
{
    private readonly DbConnectionFactory _factory;

    public AddCarEndpoint(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public override void Configure()
    {
        Post("/cars");
        Claims("dealerId");
        Description(x => x.WithName("AddCar"));
    }

    public override async Task HandleAsync(AddCarRequest req, CancellationToken ct)
    {
        using var connection = _factory.CreateConnection();
        var dealerId = int.Parse(User.ClaimValue("dealerId")!);

        var sql = @"INSERT INTO Cars (Make, Model, Year, Price, Stock, DealerId) 
                    VALUES (@Make, @Model, @Year, @Price, @Stock, @DealerId);
                    SELECT * FROM Cars WHERE Id = last_insert_rowid();";

        var car = await connection.QuerySingleAsync<CarResponse>(sql, new { req.Make, req.Model, req.Year, req.Price, req.Stock, DealerId = dealerId });
        await Send.CreatedAtAsync<GetCarEndpoint>(
            routeValues: new { id = car.Id },
            responseBody: car,
            cancellation: ct);
    }
}