using LucysLemonadeStand.Core.Interfaces;
using LucysLemonadeStand.Core.Models;
using LucysLemonadeStand.Infrastructure.DataAccess;

namespace LucysLemonadeStand.Infrastructure.Repositories;
public class PricesRepository : IPricesRepository
{
    private readonly ISqlDataAccess _db;

    public PricesRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public Task<IEnumerable<PriceEntry>> GetAllAsync()
    {
        return _db.LoadData<PriceEntry>("[dbo].[stp_GetPrices]");
    }

    public Task<PriceEntry?> GetByIdAsync(string id)
    {
        return _db.LoadSingle<PriceEntry, dynamic>("[dbo].[stp_GetPrices]", new { Item = id });
    }
}