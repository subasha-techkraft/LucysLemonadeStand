using LucysLemonadeStand.Core.Models;

namespace LucysLemonadeStand.Core.Interfaces;
public interface IPurchaseService
{
    Task<IEnumerable<string>?> VerifyPurchase(Order order);

    Task<OrderEntry> CompletePurchase(Order order);
}
