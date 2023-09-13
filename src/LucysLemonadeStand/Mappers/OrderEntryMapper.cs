using FastEndpoints;
using LucysLemonadeStand.Endpoints.V1.StandEndpoints;

namespace LucysLemonadeStand.Mappers;

public class OrderEntryMapper : Mapper<GetOrderHistoryRequest, GetOrderHistoryResponse, IEnumerable<Core.Models.OrderEntry>>
{
    public override GetOrderHistoryResponse FromEntity(IEnumerable<Core.Models.OrderEntry> coreEntries)
    {
        return new GetOrderHistoryResponse() { Orders = coreEntries.Select(FromCoreEntry) };
    }

    private OrderEntry FromCoreEntry(Core.Models.OrderEntry coreEntry)
    {
        return new OrderEntry()
        {
            Type = coreEntry.Type,
            Cups = coreEntry.Cups,
            CashGiven = coreEntry.CashGiven,
            Change = coreEntry.Change,
            Occurred = coreEntry.Occurred
        };
    }
}
