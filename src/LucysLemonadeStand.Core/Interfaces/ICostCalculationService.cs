using System;

namespace LucysLemonadeStand.Core.Interfaces;
public interface ICostCalculationService
{
    decimal CalculateCost(int cups, decimal costPerCup);
    decimal CalculateChange(int cups, decimal costPerCup, decimal cashGiven);
}
