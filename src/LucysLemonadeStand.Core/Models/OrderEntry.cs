using System;

namespace LucysLemonadeStand.Core.Models;
public class OrderEntry : Order
{
    public int ID { get; set; }

    /// <summary>
    /// 0 is normal sale, 1 is refill
    /// </summary>
    public int Type { get; set; }
    
    public decimal Change { get; set; }

    public DateTimeOffset Occurred { get; set; }
}
