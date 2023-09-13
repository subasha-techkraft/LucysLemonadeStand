using FastEndpoints;
using LucysLemonadeStand.Core.Interfaces;
using LucysLemonadeStand.Core.Models;
using System;
using System.Net;
using System.Net.Mime;

namespace LucysLemonadeStand.Endpoints.V1.StandEndpoints;

public class AskMomForAPitcher : EndpointWithoutRequest<AskMomForAPitcherResponse>
{
    private readonly IPitcherRepository _pitcherRepo;
    private readonly IPitcherRequestService _pitcherRequestService;

    public AskMomForAPitcher(IPitcherRepository pitcherRepo, IPitcherRequestService pitcherRequestService)
    {
        _pitcherRepo = pitcherRepo;
        _pitcherRequestService = pitcherRequestService;
    }

    /// <summary>
    /// This method tells the API we're building how this endpoint works. 
    /// It is a GET call that produces a <see cref="GetPitcherResponse"/>.
    /// It also specifies the documentation comments that will appear in Swagger.
    /// </summary>
    public override void Configure()
    {
        Get("/mom/refill");
        AllowAnonymous();
        Version(1);
        Description(c =>
        {
            c.Produces<AskMomForAPitcherResponse>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json);
        });
        Summary(c =>
        {
            c.Summary = "Ask mom for a pitcher of lemonade.";
            c.Description = "Ask mom for a pitcher of lemonade.";

            c.Response((int)HttpStatusCode.OK, "The amount of cups Mom gave.");
            c.ResponseExamples[(int)HttpStatusCode.OK] = new AskMomForAPitcherResponse()
            {
                Cups = 8
            };
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Pitcher? pitcher = (await _pitcherRepo.GetAllAsync()).FirstOrDefault() ?? throw new Exception("No pitcher found");
        try
        {
            pitcher.Cups += await _pitcherRequestService.RequestPitcher();
        } catch (Exception ex)
        {
            AddError(ex.Message);
            ThrowIfAnyErrors();
        }
        await SendOkAsync(new AskMomForAPitcherResponse() { Cups = pitcher.Cups }, ct);
    }
}
