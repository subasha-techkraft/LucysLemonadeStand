using LucysLemonadeStand.Core.Models;
using LucysLemonadeStand.SharedKernel.Interfaces;
using System;

namespace LucysLemonadeStand.Core.Interfaces;
public interface IOrderRepository : IRepository<OrderEntry, int>, ISimplePaginatedReadRepository<OrderEntry, int>
{ 
}