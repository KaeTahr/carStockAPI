using FastEndpoints;
using Dapper;

using Data;

public class GetCarEndpoint : Endpoint<GetCarRequest, CarResponse>
{

    private readonly DbConnectionFactory _factory;

    public GetCarEndpoint(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("/cars/id/{id:int}");
        AllowAnonymous();
        Description(x => x.WithName("Get Car")
            .WithSummary("Retrieves details of a specific car by its ID")
            .WithTags("Cars"));
    }

    public override async Task HandleAsync(GetCarRequest req, CancellationToken ct)
    {
        using var connection = _factory.CreateConnection();
        var car = await connection.QueryFirstOrDefaultAsync<CarResponse>("SELECT * FROM Cars WHERE Id = @Id", new { Id = req.Id });
        if (car == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        await Send.OkAsync(car, ct);
    }
}