using System;

namespace LucysLemonadeStand.Core.Models;
public class Pitcher
{
    public int Cups { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (Object.ReferenceEquals(this, obj))
            return true;
        if (obj is not Pitcher other)
            return false;
        return Cups == other.Cups;
    }

    public override int GetHashCode()
    {
        return Cups;
    }
}