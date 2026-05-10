using BankingSystem.Core.Enums;

namespace BankingSystem.Core.Models;

public abstract class Account
{
    protected Account()
    {
    }

    protected Account(AccountType type, decimal initialBalance)
    {
        Id = Guid.NewGuid();
        Type = type;
        Balance = initialBalance;
    }

    public Guid Id { get; private set; }
    public AccountType Type { get; private set; }
    public decimal Balance { get; private set; }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.");
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.");
        if (amount > Balance) throw new InvalidOperationException("Insufficient balance.");
        Balance -= amount;
    }
}
