namespace BankingSystem.Core.Models;

public sealed class TransactionRecord
{
    private TransactionRecord()
    {
    }

    public TransactionRecord(string action, decimal amount, Guid fromAccountId, Guid? toAccountId = null)
    {
        Id = Guid.NewGuid();
        Action = action;
        Amount = amount;
        FromAccountId = fromAccountId;
        ToAccountId = toAccountId;
        OccurredAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Action { get; }
    public decimal Amount { get; }
    public Guid FromAccountId { get; }
    public Guid? ToAccountId { get; }
    public DateTime OccurredAt { get; }
}
