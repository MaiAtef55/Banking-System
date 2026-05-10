using BankingSystem.Core.Enums;
using BankingSystem.Core.Interfaces;
using BankingSystem.Core.Models;
using BankingSystem.Core.Validation;

namespace BankingSystem.Core.Services;

public sealed class BankService : IBankService
{
    private readonly ILoggerService _logger;
    private readonly List<Customer> _customers = new();

    public BankService(ILoggerService logger)
    {
        _logger = logger;
    }

    public IReadOnlyCollection<Customer> Customers => _customers;

    public Customer AddCustomer(string name, int age, Gender gender, string address, string nationalId)
    {
        var normalizedNationalId = NationalIdValidator.NormalizeAndValidate(nationalId);
        if (_customers.Any(c => string.Equals(c.NationalId, normalizedNationalId, StringComparison.Ordinal)))
            throw new InvalidOperationException("National ID already exists.");

        var customer = new Customer(name, age, gender, address, normalizedNationalId);
        _customers.Add(customer);
        _logger.Log("Create Customer", $"Customer: {customer.Name}, National ID: {customer.NationalId}");
        return customer;
    }

    public void EditCustomer(Guid customerId, string name, int age, Gender gender, string address, string nationalId)
    {
        var customer = GetCustomer(customerId);
        var normalizedNationalId = NationalIdValidator.NormalizeAndValidate(nationalId);
        if (_customers.Any(c => c.Id != customerId && string.Equals(c.NationalId, normalizedNationalId, StringComparison.Ordinal)))
            throw new InvalidOperationException("National ID already exists.");

        customer.Update(name, age, gender, address, normalizedNationalId);
    }

    public void CloseCustomer(Guid customerId)
    {
        var customer = GetCustomer(customerId);
        customer.Close();
    }

    public Account AddAccount(Guid customerId, AccountType type, decimal initialBalance = 0)
    {
        var customer = GetCustomer(customerId);
        Account account = type switch
        {
            AccountType.Saving => new SavingAccount(initialBalance),
            AccountType.Salary => new SalaryAccount(initialBalance),
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Unsupported account type.")
        };

        customer.Accounts.Add(account);
        return account;
    }

    public void Deposit(Guid customerId, Guid accountId, decimal amount)
    {
        var customer = GetCustomer(customerId);
        var account = GetAccount(customer, accountId);
        account.Deposit(amount);

        customer.Transactions.Add(new TransactionRecord("Deposit", amount, account.Id));
        _logger.Log("Deposit", $"Customer: {customer.Name}, Amount: {amount}, Account: {account.Id}");
    }

    public void Withdraw(Guid customerId, Guid accountId, decimal amount)
    {
        var customer = GetCustomer(customerId);
        var account = GetAccount(customer, accountId);
        account.Withdraw(amount);

        customer.Transactions.Add(new TransactionRecord("Withdraw", amount, account.Id));
        _logger.Log("Withdraw", $"Customer: {customer.Name}, Amount: {amount}, Account: {account.Id}");
    }

    public void Transfer(Guid customerId, Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        var customer = GetCustomer(customerId);
        var from = GetAccount(customer, fromAccountId);
        var to = GetAccount(customer, toAccountId);

        from.Withdraw(amount);
        to.Deposit(amount);

        customer.Transactions.Add(new TransactionRecord("Transfer", amount, from.Id, to.Id));
        _logger.Log("Transfer", $"Customer: {customer.Name}, Amount: {amount}, From: {from.Id}, To: {to.Id}");
    }

    public Certificate CreateCertificate(Guid customerId, CertificatePeriod period, decimal price)
    {
        var customer = GetCustomer(customerId);
        var certificate = new Certificate(period, price);
        customer.Certificates.Add(certificate);
        customer.ServiceActivities.Add($"Certificate created ({period}) with price {price}");

        _logger.Log("Create Certificate", $"Customer: {customer.Name}, Period: {period}, Price: {price}");
        return certificate;
    }

    public void UpdateCertificate(Guid customerId, Guid certificateId, CertificatePeriod period, decimal price)
    {
        var customer = GetCustomer(customerId);
        var certificate = customer.Certificates.FirstOrDefault(x => x.Id == certificateId)
            ?? throw new InvalidOperationException("Certificate not found.");

        certificate.Update(period, price);
        customer.ServiceActivities.Add($"Certificate updated ({period}) with price {price}");
    }

    public void DeleteCertificate(Guid customerId, Guid certificateId)
    {
        var customer = GetCustomer(customerId);
        var certificate = customer.Certificates.FirstOrDefault(x => x.Id == certificateId)
            ?? throw new InvalidOperationException("Certificate not found.");

        customer.Certificates.Remove(certificate);
        customer.ServiceActivities.Add("Certificate deleted");
    }

    public CreditCard CreateCreditCard(Guid customerId, int limit)
    {
        var customer = GetCustomer(customerId);
        var card = new CreditCard(limit);
        customer.AddCreditCard(card);

        return card;
    }

    public void UpdateCreditCardLimit(Guid customerId, int newLimit)
    {
        var customer = GetCustomer(customerId);
        var card = customer.CreditCard ?? throw new InvalidOperationException("Customer has no credit card.");

        card.UpdateLimit(newLimit);
        customer.ServiceActivities.Add($"Credit card limit updated to {newLimit}");
        _logger.Log("Update Credit Card", $"Customer: {customer.Name}, Limit: {newLimit}");
    }

    public CustomerReport GenerateCustomerReport(Guid customerId)
    {
        var customer = GetCustomer(customerId);
        var totalBalance = customer.Accounts.Sum(x => x.Balance);

        return new CustomerReport
        {
            CustomerName = customer.Name,
            FinalBalance = totalBalance,
            Transactions = customer.Transactions
                .Select(x => $"{x.OccurredAt:yyyy-MM-dd HH:mm} - {x.Action} - {x.Amount}")
                .ToList(),
            ServiceActivities = customer.ServiceActivities.ToList()
        };
    }

    public BankStatistics GenerateBankStatistics()
    {
        return new BankStatistics
        {
            TotalCustomers = _customers.Count,
            TotalAssets = _customers.Sum(c => c.Accounts.Sum(a => a.Balance) + c.Certificates.Sum(s => s.Price)),
            TotalCertificates = _customers.Sum(c => c.Certificates.Count),
            TotalCreditCards = _customers.Count(c => c.CreditCard is not null)
        };
    }

    private Customer GetCustomer(Guid customerId)
    {
        return _customers.FirstOrDefault(x => x.Id == customerId)
            ?? throw new InvalidOperationException("Customer not found.");
    }

    private static Account GetAccount(Customer customer, Guid accountId)
    {
        return customer.Accounts.FirstOrDefault(x => x.Id == accountId)
            ?? throw new InvalidOperationException("Account not found.");
    }
}
