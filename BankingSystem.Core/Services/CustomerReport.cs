namespace BankingSystem.Core.Services;

public sealed class CustomerReport
{
    public string CustomerName { get; init; } = string.Empty;
    public decimal FinalBalance { get; init; }
    public IReadOnlyCollection<string> Transactions { get; init; } = Array.Empty<string>();
    public IReadOnlyCollection<string> ServiceActivities { get; init; } = Array.Empty<string>();
}
