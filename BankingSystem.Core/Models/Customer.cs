using BankingSystem.Core.Enums;
using BankingSystem.Core.Validation;

namespace BankingSystem.Core.Models;

public sealed class Customer
{
    private Customer()
    {
    }

    public Customer(string name, int age, Gender gender, string address, string nationalId)
    {
        Id = Guid.NewGuid();
        Update(name, age, gender, address, nationalId);
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Age { get; private set; }
    public Gender Gender { get; private set; }
    public string Address { get; private set; } = string.Empty;
    public string NationalId { get; private set; } = string.Empty;
    public bool IsClosed { get; private set; }
    public decimal TotalAmount { get; private set; }

    public List<Account> Accounts { get; } = new();
    public List<Certificate> Certificates { get; } = new();
    public CreditCard? CreditCard { get; private set; }
    public List<TransactionRecord> Transactions { get; } = new();
    public List<string> ServiceActivities { get; } = new();

    public void Update(string name, int age, Gender gender, string address, string nationalId)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(ValidationMessages.MandatoryField);
        if (age <= 0) throw new ArgumentException("Age must be greater than zero.");
        if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException(ValidationMessages.MandatoryField);

        var normalizedNationalId = NationalIdValidator.NormalizeAndValidate(nationalId);

        Name = name.Trim();
        Age = age;
        Gender = gender;
        Address = address.Trim();
        NationalId = normalizedNationalId;
    }

    public void Close() => IsClosed = true;

    public void RecalculateTotalAmount()
    {
        TotalAmount = Accounts.Sum(a => a.Balance);
    }

    public void AddCreditCard(CreditCard card)
    {
        if (CreditCard is not null) throw new InvalidOperationException("Only one credit card is allowed per customer.");
        CreditCard = card;
        ServiceActivities.Add($"Credit card created with limit {card.CashLimit}");
    }
}
