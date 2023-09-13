using LucysLemonadeStand.Core.Interfaces;
using LucysLemonadeStand.Core.Models;
using LucysLemonadeStand.SharedKernel;
using System;

namespace LucysLemonadeStand.Core.Services;
public class PurchaseService : IPurchaseService
{
    private readonly IPitcherRepository _pitcherRepository;
    private readonly IPricesRepository _priceRepository;
    private readonly ICostCalculationService _costCalculationService;
    private readonly IOrderRepository _orderRepository;
    private readonly DateTimeProvider _dateTimeProvider;


    public PurchaseService(IPitcherRepository pitcherRepository, 
        IPricesRepository priceRepository,
        ICostCalculationService costCalculationService,
        IOrderRepository orderRepository,
        DateTimeProvider dateTimeProvider)
    {
        _pitcherRepository = pitcherRepository;
        _priceRepository = priceRepository;
        _costCalculationService = costCalculationService;
        _orderRepository = orderRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<OrderEntry> CompletePurchase(Order order)
    {
        decimal pricePerCup = (await _priceRepository.GetAllAsync()).Single(p => p.Item == "Cup").Price;
        OrderEntry orderEntry = new()
        {
            Type = 0,
            Cups = order.Cups,
            CashGiven = order.CashGiven,
            Change = _costCalculationService.CalculateChange(order.Cups, pricePerCup, order.CashGiven),
            Occurred = _dateTimeProvider.DateTime
        };
        int orderID = await _orderRepository.InsertAsync(orderEntry);
        orderEntry.ID = orderID;
        return orderEntry;
    }

    public async Task<IEnumerable<string>?> VerifyPurchase(Order order)
    {
        //check the following cases:
        //0. that the order makes sense. I.e. no values <= 0 for cups and cash
        //1. that the requested amount of cups is available
        //2. that the cash given is at least the cost of the requested cups
        //if we dealt with different denominations of cash, we could also check that we can make change.
        //We're assuming that all payments are in pennies to avoid coding all that change logic.
        List<string> errors = new();
        if (order.Cups <= 0)
            errors.Add("Requested cups must be positive.");
        if (order.CashGiven <= 0)
            errors.Add("Cash given must be positive.");
        if (errors.Any())
            return errors;

        int cupsAvailable = (await _pitcherRepository.GetAllAsync()).Single().Cups;
        if (cupsAvailable < order.Cups)
        {
            errors.Add($"The order of {order.Cups} cups cannot be fulfilled because the pitcher only has {cupsAvailable} cups.");
            return errors;
        }
        decimal pricePerCup = (await _priceRepository.GetAllAsync()).Single(p => p.Item == "Cup").Price;
        decimal totalCost = _costCalculationService.CalculateCost(order.Cups, pricePerCup);
        if (order.CashGiven < totalCost)
        {
            errors.Add($"The cash given (${order.CashGiven:N2}) does not cover the cost of the cups requested (${totalCost:N2}).");
            return errors;
        }
        return errors;
    }
}
