namespace BankingSystem.Core.Services;

public sealed class BankStatistics
{
    public int TotalCustomers { get; init; }
    public decimal TotalAssets { get; init; }
    public int TotalCertificates { get; init; }
    public int TotalCreditCards { get; init; }
}
