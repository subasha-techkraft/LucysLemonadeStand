using LucysLemonadeStand.Core.Interfaces;
using LucysLemonadeStand.Core.Models;
using LucysLemonadeStand.Infrastructure.DataAccess;
using LucysLemonadeStand.SharedKernel.Pagination;

namespace LucysLemonadeStand.Infrastructure.Repositories;
public class OrderRepository : IOrderRepository
{
    private readonly ISqlDataAccess _db;

    public OrderRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public Task<IEnumerable<OrderEntry>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<OrderEntry?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderEntry>> GetPageAsync(SimplePaginationSettings paginationSettings)
    {
        return _db.LoadData<OrderEntry, SimplePaginationSettings>("[dbo].[stp_GetOrderHistory]", paginationSettings);
    }

    public Task<int> InsertAsync(OrderEntry record)
    {
        return _db.SaveRecordAndGetID<dynamic, int>("[dbo].[stp_SaveOrder]", new {
            record.Type,
            record.Cups,
            record.CashGiven,
            record.Change,
            record.Occurred
        }, "@OrderID");
    }

    public Task<IEnumerable<int>> InsertAsync(IEnumerable<OrderEntry> records)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(OrderEntry record)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(IEnumerable<OrderEntry> records)
    {
        throw new NotImplementedException();
    }
}