using LucysLemonadeStand.Core.Interfaces;
using System;

namespace LucysLemonadeStand.Core.Services;
public class CostCalculationService : ICostCalculationService
{
    public decimal CalculateChange(int cups, decimal costPerCup, decimal cashGiven)
    {
        return cashGiven - CalculateCost(cups, costPerCup);
    }

    public decimal CalculateCost(int cups, decimal costPerCup)
    {
        return cups * costPerCup;
    }
}
