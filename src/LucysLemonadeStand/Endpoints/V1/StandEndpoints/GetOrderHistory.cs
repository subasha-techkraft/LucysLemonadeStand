using FastEndpoints;
using LucysLemonadeStand.Mappers;
using System.Net.Mime;
using System.Net;
using LucysLemonadeStand.Core.Interfaces;

namespace LucysLemonadeStand.Endpoints.V1.StandEndpoints;

public class GetOrderHistory : Endpoint<GetOrderHistoryRequest, GetOrderHistoryResponse, OrderEntryMapper>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderHistory(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public override void Configure()
    {
        Get(GetOrderHistoryRequest.Route);
        AllowAnonymous();
        Version(1);
        Description(c =>
        {
            c.Produces<GetOrderHistoryResponse>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json);
        });
        Summary(c =>
        {
            c.Summary = "Get the history of purchase orders.";
            c.Description = "Get the history of purchase orders.";
            c.Response((int)HttpStatusCode.OK, "The orders.");
            c.ResponseExamples[(int)HttpStatusCode.OK] = new GetOrderHistoryResponse()
            {
                Orders = new List<OrderEntry>()
                {
                    new OrderEntry()
                    {
                        Type = 0,
                        Cups = 3,
                        CashGiven = 2m,
                        Change = 0.5m,
                        Occurred = DateTimeOffset.FromUnixTimeSeconds(int.MaxValue)
                    }
                }
            };
        });
    }

    public override async Task HandleAsync(GetOrderHistoryRequest req, CancellationToken ct)
    {
        var orders = await _orderRepository.GetPageAsync(req);
        var response = Map.FromEntity(orders);
        await SendAsync(response, cancellation: ct);
    }
}
