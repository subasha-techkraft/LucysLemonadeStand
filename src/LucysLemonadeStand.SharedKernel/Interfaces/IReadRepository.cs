namespace LucysLemonadeStand.SharedKernel.Interfaces;

public interface IReadRepository<T, TId> where T : class
{
    /// <summary>
    /// Get all the records.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Get one record by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<T?> GetByIdAsync(TId id);
}
