namespace BankingSystem.Core.Models;

public sealed class CreditCard
{
    public const int MinLimit = 50000;
    public const int MaxLimit = 250000;

    private CreditCard()
    {
    }

    public CreditCard(int cashLimit)
    {
        ValidateLimit(cashLimit);

        Id = Guid.NewGuid();
        CashLimit = cashLimit;
        ExpiryDate = DateTime.UtcNow.AddYears(10);
    }

    public Guid Id { get; private set; }
    public int CashLimit { get; private set; }
    public DateTime ExpiryDate { get; }

    public void UpdateLimit(int newLimit)
    {
        ValidateLimit(newLimit);
        CashLimit = newLimit;
    }

    private static void ValidateLimit(int limit)
    {
        if (limit < MinLimit || limit > MaxLimit)
            throw new ArgumentException("Credit card limit must be between 50,000 and 250,000.");
    }
}
