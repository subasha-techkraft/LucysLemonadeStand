using FastEndpoints;
using System.Net.Mime;
using System.Net;
using LucysLemonadeStand.Core.Interfaces;
using LucysLemonadeStand.Mappers;

namespace LucysLemonadeStand.Endpoints.V1.StandEndpoints;

public class Buy : Endpoint<BuyRequest, BuyResponse, BuyRequestToOrderMapper>
{
    private readonly IPurchaseService _purchaseService;

    public Buy(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }


    /// <summary>
    /// This method tells the API we're building how this endpoint works. 
    /// It is a GET call that takes in a <see cref="BuyRequest"/> and produces a <see cref="BuyResponse"/>.
    /// It also specifies the documentation comments that will appear in Swagger.
    /// </summary>
    public override void Configure()
    {
        Post(BuyRequest.Route);
        AllowAnonymous();
        Version(1);
        Description(c =>
        {
            c.Produces<BuyResponse>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json);
        });
        Summary(c =>
        {
            c.Summary = "Buy some cups of lemonade and get change.";
            c.Description = "Buy some cups of lemonade and get change.";
            c.ExampleRequest = new BuyRequest()
            {
                Cups = 3,
                Cash = 2.00m
            };
            c.Response((int)HttpStatusCode.OK, "An object containing the uptime timespan");
            c.Response((int)HttpStatusCode.BadRequest, "If there's not enough lemonade to sell, if there's not enough cash on hand to make change, or if the cash given is not enough to pay for the cups requested.");
            c.ResponseExamples[(int)HttpStatusCode.OK] = new BuyResponse() 
            { 
                Cups = 3,
                Change = 0.50m
            };
        });
    }

    public override async Task HandleAsync(BuyRequest req, CancellationToken ct)
    {
        Core.Models.Order order = Map.ToEntity(req);
        IEnumerable<string>? errors = await _purchaseService.VerifyPurchase(order);
        foreach(string error in errors ?? Array.Empty<string>())
        {
            AddError(error);
        }
        ThrowIfAnyErrors();
        Core.Models.OrderEntry orderEntry = await _purchaseService.CompletePurchase(order);
            
        await SendOkAsync(new BuyResponse() { Cups = orderEntry.Cups, Change = orderEntry.Change }, ct);
    }
}
