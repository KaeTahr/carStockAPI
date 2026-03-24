using Dapper;
using FastEndpoints;
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
        AllowAnonymous(); // TODO: add JWT
        Description(x => x.WithName("AddCar"));
    }

    public override async Task HandleAsync(AddCarRequest req, CancellationToken ct)
    {
        using var connection = _factory.CreateConnection();

        var sql = @"INSERT INTO Cars (Make, Model, Year, Price, Stock, DealerId) 
                    VALUES (@Make, @Model, @Year, @Price, @Stock, 1);
                    SELECT * FROM Cars WHERE Id = last_insert_rowid();";

        var car = await connection.QuerySingleAsync<CarResponse>(sql, req);
        await Send.CreatedAtAsync<GetCarEndpoint>(
            routeValues: new { id = car.Id },
            responseBody: car,
            cancellation: ct);
    }
}