namespace LucysLemonadeStand.SharedKernel;
public class DateTimeProvider
{
    public DateTime DateTime { get => this.DateTimeOffset.DateTime; }
    public DateTimeOffset DateTimeOffset { get; init; }

    public DateTimeProvider() => DateTimeOffset = DateTimeOffset.Now;

    public DateTimeProvider(DateTimeOffset dateTimeOffset) => DateTimeOffset = dateTimeOffset;
}
