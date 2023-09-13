using FastEndpoints;
using LucysLemonadeStand.Core.Interfaces;
using LucysLemonadeStand.Core.Models;
using System.Net;
using System.Net.Mime;
using System.Linq;
using LucysLemonadeStand.Endpoints.V1.StandEndpoints;

namespace LucysLemonadeStand.Endpoints.V1.StandEndpoints;

public class GetPrices : Endpoint<GetPricesRequest,List<GetPricesResponse>>
{
    private readonly IPricesRepository _priceEntryRepository;

    public GetPrices(IPricesRepository priceEntryRepository)
    {
        _priceEntryRepository = priceEntryRepository;
    }

    /// <summary>
    /// This method tells the API we're building how this endpoint works. 
    /// It is a GET call that produces a <see cref="List<GetPricesResponse>"/>.
    /// It also specifies the documentation comments that will appear in Swagger.
    /// </summary>
    public override void Configure()
    {
        Get(GetPricesRequest.Route);
        AllowAnonymous();
        Version(1);
        Description(c =>
        {
            c.Produces<List<GetPricesResponse>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json);
        });
        Summary(c =>
        {
            c.Summary = "Get price per cup of an item.";
            c.Description = "Get price per cup of an item.";
            c.ExampleRequest = new GetPricesRequest()
            {
                Item = "Cup",
            };
            c.Response((int)HttpStatusCode.OK, "An object containing price of the item.");
            c.ResponseExamples[(int)HttpStatusCode.OK] = new List<GetPricesResponse>()
            {
                new GetPricesResponse()
                {
                    Item = "Cup",
                    Price = 0.05m
                }, 
                new GetPricesResponse()
                {
                    Item = "Refill of 8 cups",
                    Price = 3.20m
                }
            };
        });
    }

    public override async Task HandleAsync(GetPricesRequest getPricesRequest, CancellationToken ct)
    {
        List<PriceEntry> priceEntries = new();
        if (getPricesRequest.Item is null)
        {
            priceEntries = (await _priceEntryRepository.GetAllAsync()).ToList();
        }
        else
        {
            var entry = await _priceEntryRepository.GetByIdAsync(getPricesRequest.Item);
            if (entry is not null)
                priceEntries.Add(entry);
        }
        List<GetPricesResponse> getPricesResponses = priceEntries.Select(x=>new GetPricesResponse() { Item = x.Item, Price = x.Price }).ToList();
        await SendOkAsync(getPricesResponses, ct);
    }
}   