namespace LucysLemonadeStand.SharedKernel.Interfaces;

public interface IWriteRepository<T, TId> where T : class
{
    /// <summary>
    /// Insert one record.
    /// </summary>
    /// <param name="record"></param>
    /// <returns>The ID of the created record.</returns>
    Task<TId> InsertAsync(T record);

    /// <summary>
    /// Update one record.
    /// </summary>
    /// <param name="record"></param>
    /// <returns>True if updating was successful, false if no rows were modified.</returns>
    Task<bool> UpdateAsync(T record);

    /// <summary>
    /// Insert a collection of records.
    /// </summary>
    /// <param name="records"></param>
    /// <returns>The IDs of the created records.</returns>
    Task<IEnumerable<TId>> InsertAsync(IEnumerable<T> records);

    /// <summary>
    /// Update a collection of records.
    /// </summary>
    /// <param name="records"></param>
    /// <returns>The amount of records that were modified.</returns>
    Task<int> UpdateAsync(IEnumerable<T> records);
}
