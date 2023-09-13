using LucysLemonadeStand.SharedKernel.Pagination;

namespace LucysLemonadeStand.SharedKernel.Interfaces;
public interface ISimplePaginatedReadRepository<T, TId> where T : class
{
    Task<IEnumerable<T>> GetPageAsync(SimplePaginationSettings paginationSettings);
}
