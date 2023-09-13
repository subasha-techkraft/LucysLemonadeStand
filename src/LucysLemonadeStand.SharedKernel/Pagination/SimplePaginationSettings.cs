using System;

namespace LucysLemonadeStand.SharedKernel.Pagination;
public record SimplePaginationSettings
{
    /// <summary>
    /// The 0-based page number. The query to get records will return up to <see cref="PageSize"/> records after skipping the first <see cref="Page"/> * <see cref="PageSize"/> records. 
    /// </summary>
    public int? Page { get; set; } = 0;

    /// <summary>
    /// The maximum amount of records to return.
    /// </summary>
    public int? PageSize { get; set; } = 50;
}
