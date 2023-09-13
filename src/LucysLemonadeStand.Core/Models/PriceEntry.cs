using System;

namespace LucysLemonadeStand.Core.Models;
public class PriceEntry
{
    public string Item { get; set; } = string.Empty;
    public decimal Price { get; set; }
}