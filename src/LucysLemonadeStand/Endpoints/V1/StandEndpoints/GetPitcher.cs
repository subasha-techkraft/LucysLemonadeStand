using FastEndpoints;
using LucysLemonadeStand.Core.Interfaces;
using LucysLemonadeStand.Core.Models;
using System.Net;
using System.Net.Mime;

namespace LucysLemonadeStand.Endpoints.V1.StandEndpoints;

public class GetPitcher : EndpointWithoutRequest<GetPitcherResponse>
{
    private readonly IPitcherRepository _pitcherRepo;

    public GetPitcher(IPitcherRepository pitcherRepo)
    {
        _pitcherRepo = pitcherRepo;
    }

    /// <summary>
    /// This method tells the API we're building how this endpoint works. 
    /// It is a GET call that produces a <see cref="GetPitcherResponse"/>.
    /// It also specifies the documentation comments that will appear in Swagger.
    /// </summary>
    public override void Configure()
    {
        Get("/pitcher");
        AllowAnonymous();
        Version(1);
        Description(c =>
        {
            c.Produces<GetPitcherResponse>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json);
        });
        Summary(c =>
        {
            c.Summary = "Get the amount of cups Lucy can still sell.";
            c.Description = "Get the amount of cups Lucy can still sell.";

            c.Response((int)HttpStatusCode.OK, "The amount of cups Lucy can still sell.");
            c.ResponseExamples[(int)HttpStatusCode.OK] = new GetPitcherResponse()
            {
                Cups = 8
            };
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Pitcher? pitcher = (await _pitcherRepo.GetAllAsync()).FirstOrDefault();
        if (pitcher is null)
        {
            throw new Exception("No pitcher found");
        }
        await SendOkAsync(new GetPitcherResponse() { Cups = pitcher.Cups }, ct);
    }
}
