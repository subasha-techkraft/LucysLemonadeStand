using LucysLemonadeStand.Core.Models;
using LucysLemonadeStand.SharedKernel.Interfaces;

namespace LucysLemonadeStand.Core.Interfaces;
public interface IPricesRepository : IReadRepository<PriceEntry, string>
{
}