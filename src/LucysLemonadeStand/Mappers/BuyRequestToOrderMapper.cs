using FastEndpoints;
using LucysLemonadeStand.Core.Models;
using LucysLemonadeStand.Endpoints.V1.StandEndpoints;

namespace LucysLemonadeStand.Mappers;

public class BuyRequestToOrderMapper : RequestMapper<BuyRequest, Core.Models.Order>
{
    public override Core.Models.Order ToEntity(BuyRequest buyRequest)
    {
        return new Core.Models.Order()
        {
            Cups = buyRequest.Cups,
            CashGiven = buyRequest.Cash
        };
    }
}
