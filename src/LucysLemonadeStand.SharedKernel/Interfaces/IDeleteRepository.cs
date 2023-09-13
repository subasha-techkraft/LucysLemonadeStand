namespace LucysLemonadeStand.SharedKernel.Interfaces;

public interface IDeleteRepository<TId>
{
    Task DeleteAsync(TId id);
}
