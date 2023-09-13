namespace LucysLemonadeStand.SharedKernel.Pagination;
public record PaginationSettings : SimplePaginationSettings
{
  public bool? Ascending { get; set; } = true;
  public string? Filter { get; set; }
  public FieldFilter[]? Filters { get; set; }
  public FieldOrder[]? OrderBy { get; set; }

  public static PaginationSettings Default { get; } = new PaginationSettings() 
  {
    Page = 0, PageSize = 50
  };
}
