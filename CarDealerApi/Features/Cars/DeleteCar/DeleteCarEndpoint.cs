using FastEndpoints;
using Dapper;

using Data;

public class DeleteCarEndpoint : Endpoint<GetCarRequest>
{
    private readonly DbConnectionFactory _factory;

    public DeleteCarEndpoint(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public override void Configure()
    {
        Verbs(Http.DELETE);
        Routes("/cars/{id}");
        AllowAnonymous();
        Description(x => x.WithName("Delete Car")
            .WithSummary("Deletes a car from the inventory")
            .WithTags("Cars"));
    }

    public override async Task HandleAsync(GetCarRequest req, CancellationToken ct)
    {
        using var connection = _factory.CreateConnection();
        var affectedRows = await connection.ExecuteAsync("DELETE FROM Cars WHERE Id = @Id", new { Id = req.Id });
        if (affectedRows == 0)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        await Send.OkAsync(ct);
    }
}