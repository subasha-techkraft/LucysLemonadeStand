namespace LucysLemonadeStand.SharedKernel.Interfaces;

// from Ardalis.Specification
public interface IRepository<T, TId> : IReadRepository<T, TId>, IWriteRepository<T, TId> where T : class
{

}
