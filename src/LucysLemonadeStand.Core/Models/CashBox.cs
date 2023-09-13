using System;

namespace LucysLemonadeStand.Core.Models;
public class CashBox
{
    public decimal CashOnHand { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (Object.ReferenceEquals(this, obj))
            return true;
        if (obj is not CashBox other) 
            return false;
        return CashOnHand == other.CashOnHand;
    }

    public override int GetHashCode()
    {
        return CashOnHand.GetHashCode();
    }
}