using FastEndpoints;
using LucysLemonadeStand.Core.Interfaces;
using System.Net;
using System.Net.Mime;

namespace LucysLemonadeStand.Endpoints.V1.StandEndpoints;

public class CashBox : EndpointWithoutRequest<CashBoxResponse>
{
    private readonly ICashBoxRepository _cashBoxRepository;

    public CashBox(ICashBoxRepository cashBoxRepository)
    {
        _cashBoxRepository= cashBoxRepository;
    }


    /// <summary>
    /// This method tells the API we're building how this endpoint works. 
    /// It is a GET call that produces a <see cref="CashBoxResponse"/>.
    /// It also specifies the documentation comments that will appear in Swagger.
    /// </summary>
    public override void Configure()
    {
        Get("/cashbox");
        AllowAnonymous();
        Version(1);
        Description(c =>
        {
            c.Produces<CashBoxResponse>((int)HttpStatusCode.OK,MediaTypeNames.Application.Json);
        });
        Summary(c =>
        {
            c.Summary = "Get the total amount in the cashbox.";
            c.Description = "Get the total amount in the cashbox.";
            c.Response((int)HttpStatusCode.OK,"The total amount in the cashbox.");
            c.ResponseExamples[(int)HttpStatusCode.OK] = new CashBoxResponse()
            {
                CashOnHand = 550.00m
            };
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            decimal cashOnHand = (await _cashBoxRepository.GetAllAsync()).First().CashOnHand;
            await SendOkAsync(new CashBoxResponse() { CashOnHand=cashOnHand },ct);
        } catch (Exception ex)
        {
            throw new Exception("Missing cashbox!", ex);
        }
    }

}
