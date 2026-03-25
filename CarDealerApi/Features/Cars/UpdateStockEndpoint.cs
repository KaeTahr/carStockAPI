using Dapper;
using FastEndpoints;
using Data;

namespace Features.Cars;

public class UpdateStockEndpoint : Endpoint<UpdateStockRequest, CarResponse>
{
    private readonly DbConnectionFactory _factory;

    public UpdateStockEndpoint(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public override void Configure()
    {
        // We use PATCH because requirements only specify "update stock", not "update entire car"
        Patch("/cars/{id:int}/stock");
        AllowAnonymous(); // TODO: JWT
        Description(x => x.WithName("UpdateStock"));
    }

    public override async Task HandleAsync(UpdateStockRequest req, CancellationToken ct)
    {
        var id = Route<int>("id");
        using var connection = _factory.CreateConnection();
        var sql = "UPDATE Cars SET Stock = @Stock WHERE Id = @Id;";

        var affected = await connection.ExecuteAsync(
            sql,
            new { req.Stock, Id = id });
        
        if (affected == 0)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var car = await connection.QuerySingleAsync<CarResponse>(
            "SELECT * FROM Cars WHERE Id = @Id",
            new { Id = id });

        await Send.OkAsync(car, ct);
    }
}