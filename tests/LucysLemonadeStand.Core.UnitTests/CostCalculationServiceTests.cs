using FluentAssertions;
using LucysLemonadeStand.Core.Services;
using System;
using Xunit;

namespace LucysLemonadeStand.Core.UnitTests;
public class CostCalculationServiceTests
{
    #region CalculateCost tests. These tests are the bare minimum to be called unit tests.
    //These two methods test the same thing using two different libraries.
    //They are very poor tests since they show a severe confirmation bias - they assume that since one test passes, they all pass.
    [Fact]
    public void CalculateCost_ReturnsCorrectCost_XunitAssertions()
    {
        CostCalculationService costCalculationService = new();
        Assert.Equal(3.5m, costCalculationService.CalculateCost(7, 0.5m));
    }

    [Fact]
    public void CalculateCost_ReturnsCorrectCost_FluentAssertions()
    {
        CostCalculationService costCalculationService = new();
        costCalculationService.CalculateCost(7, 0.5m).Should().Be(3.5m);
    }
    #endregion

    #region Basic CalculateChange tests. These are decent tests since they try out more parameters and a couple edge cases
    [Fact]
    public void CalculateChange_ReturnCorrectChange_xUnitAssertions()
    {
        CostCalculationService costCalculationService = new();
        Assert.Equal(0, costCalculationService.CalculateChange(cups: 1, costPerCup: 0.5m, cashGiven: 0.5m));
        Assert.Equal(0, costCalculationService.CalculateChange(2, 0.5m, 1m));
        Assert.Equal(0.5m, costCalculationService.CalculateChange(1, 0.5m, 1m));
        Assert.Equal(0, costCalculationService.CalculateChange(10, 0.5m, 5m));
        Assert.Equal(5m, costCalculationService.CalculateChange(10, 0.5m, 10m));
        Assert.Equal(1m, costCalculationService.CalculateChange(0, 10m, 1m));
        Assert.Equal(-9m, costCalculationService.CalculateChange(1, 10m, 1m));
        Assert.Equal(3.2m, costCalculationService.CalculateChange(-8, 0.4m, 0));
    }

    [Fact]
    public void CalculateChange_ReturnCorrectChange_FluentAssertions()
    {
        CostCalculationService costCalculationService = new();
        costCalculationService.CalculateChange(cups: 1, costPerCup: 0.5m, cashGiven: 0.5m).Should().Be(0);
        costCalculationService.CalculateChange(2, 0.5m, 1m).Should().Be(0);
        costCalculationService.CalculateChange(1, 0.5m, 1m).Should().Be(0.5m);
        costCalculationService.CalculateChange(10, 0.5m, 5m).Should().Be(0);
        costCalculationService.CalculateChange(10, 0.5m, 10m).Should().Be(5m);
        costCalculationService.CalculateChange(0, 10m, 1m).Should().Be(1m);
        costCalculationService.CalculateChange(1, 10m, 1m).Should().Be(-9m);
        costCalculationService.CalculateChange(-8, 0.4m, 0).Should().Be(3.2m, "hypothetically, returns should be allowed");
    }
    #endregion

    #region Fancy CalculateChange tests. These use randomized data and test the properties of the business logic.
    //When used in conjunction with the basic ones above, this is a very powerful tool because
    //it helps ensure that a lazy developer can't avoid coding a proper implementation
    //by simply writing the test cases' expected values into the implementation

    public static readonly IEnumerable<object[]> _randomOrderPairs = Enumerable.Range(1, 100).Select(x => new object[] 
    { 
        Random.Shared.Next(0, int.MaxValue / 2), //cups1
        Random.Shared.Next(0, int.MaxValue / 2), //cups2
        (decimal)(Random.Shared.NextDouble() * 100), //cost per cup
        (decimal)(Random.Shared.NextDouble() * int.MaxValue), //cashGiven1
        (decimal)(Random.Shared.NextDouble() * int.MaxValue) //cashGiven2
    });

    [Theory]
    [MemberData(nameof(_randomOrderPairs))]
    public void CalculateChange_IsConsistent_BetweenCalculatingTwoOrdersAndASumOfTwoOrders(int cups1, int cups2, decimal costPerCup, decimal cashGiven1, decimal cashGiven2)
    {
        CostCalculationService sut = new();
        
        (sut.CalculateChange(cups1, costPerCup, cashGiven1) + sut.CalculateChange(cups2, costPerCup, cashGiven2))
            .Should().Be(sut.CalculateChange(cups1 + cups2, costPerCup, cashGiven1 + cashGiven2));
    }

    public static readonly IEnumerable<object[]> _randomCostPerCupAndCashGiven = Enumerable.Range(1, 100).Select(x => new object[]
    {
        (decimal)(Random.Shared.NextDouble() * 100), //cost per cup
        (decimal)(Random.Shared.NextDouble() * int.MaxValue), //cashGiven
    });

    [Theory]
    [MemberData(nameof(_randomCostPerCupAndCashGiven))]
    public void CalculateChange_ReturnsCashGiven_WhenCupsIs0(decimal costPerCup, decimal cashGiven)
    {
        CostCalculationService sut = new();
        sut.CalculateChange(cups: 0, costPerCup, cashGiven).Should().Be(cashGiven);
    }

    public static readonly IEnumerable<object[]> _randomCupsAndCostPerCup = Enumerable.Range(1, 100).Select(x => new object[]
    {
        Random.Shared.Next(0, int.MaxValue / 2), //cups1
        (decimal)(Random.Shared.NextDouble() * 100), //cost per cup
    });

    [Theory]
    [MemberData(nameof(_randomCupsAndCostPerCup))]
    public void CalculateChange_ReturnsNegativeCost_WhenCashGivenIs0(int cups, decimal costPerCup)
    {
        CostCalculationService sut = new();
        sut.CalculateChange(cups, costPerCup, 0).Should().Be(sut.CalculateCost(cups, costPerCup) * -1);
    }
    #endregion
}
