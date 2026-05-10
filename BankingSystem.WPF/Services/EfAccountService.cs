using BankingSystem.Core.Enums;
using BankingSystem.Core.Interfaces;
using BankingSystem.Core.Logging;
using BankingSystem.Core.Models;
using BankingSystem.WPF.Db;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.WPF.Services;

public sealed class EfAccountService : IAccountService
{
    private readonly DbContextOptions<BankingSystemDbContext> _dbOptions;
    private readonly ILoggerService _logger;

    public EfAccountService(DbContextOptions<BankingSystemDbContext> dbOptions, ILoggerService logger)
    {
        _dbOptions = dbOptions;
        _logger = logger;
    }

    public Account AddAccount(Guid customerId, AccountType type, decimal initialBalance = 0)
    {
        using var db = new BankingSystemDbContext(_dbOptions);

        var customer = db.Customers
            .Include(c => c.Accounts)
            .FirstOrDefault(c => c.Id == customerId)
            ?? throw new InvalidOperationException("Customer not found.");

        Account account = type switch
        {
            AccountType.Saving => new SavingAccount(initialBalance),
            AccountType.Salary => new SalaryAccount(initialBalance),
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Unsupported account type.")
        };

        customer.Accounts.Add(account);
        customer.RecalculateTotalAmount();
        db.SaveChanges();
        return account;
    }

    public void Deposit(Guid customerId, Guid accountId, decimal amount)
    {
        using var db = new BankingSystemDbContext(_dbOptions);

        var customer = LoadCustomerWithAccountsAndTransactions(db, customerId);
        var account = customer.Accounts.FirstOrDefault(a => a.Id == accountId)
            ?? throw new InvalidOperationException("Account not found.");

        account.Deposit(amount);
        customer.Transactions.Add(new TransactionRecord("Deposit", amount, account.Id));
        customer.RecalculateTotalAmount();
        db.SaveChanges();

        _logger.Log("Deposit", $"CustomerId: {customer.Id}, Amount: {amount}, Account: {account.Id}");
    }

    public void Withdraw(Guid customerId, Guid accountId, decimal amount)
    {
        using var db = new BankingSystemDbContext(_dbOptions);

        var customer = LoadCustomerWithAccountsAndTransactions(db, customerId);
        var account = customer.Accounts.FirstOrDefault(a => a.Id == accountId)
            ?? throw new InvalidOperationException("Account not found.");

        account.Withdraw(amount);
        customer.Transactions.Add(new TransactionRecord("Withdraw", amount, account.Id));
        customer.RecalculateTotalAmount();
        db.SaveChanges();

        _logger.Log("Withdraw", $"CustomerId: {customer.Id}, Amount: {amount}, Account: {account.Id}");
    }

    public void Transfer(Guid customerId, Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        using var db = new BankingSystemDbContext(_dbOptions);

        var customer = LoadCustomerWithAccountsAndTransactions(db, customerId);
        var from = customer.Accounts.FirstOrDefault(a => a.Id == fromAccountId)
            ?? throw new InvalidOperationException("Account not found.");
        var to = customer.Accounts.FirstOrDefault(a => a.Id == toAccountId)
            ?? throw new InvalidOperationException("Account not found.");

        from.Withdraw(amount);
        to.Deposit(amount);
        customer.Transactions.Add(new TransactionRecord("Transfer", amount, from.Id, to.Id));
        customer.RecalculateTotalAmount();
        db.SaveChanges();

        _logger.Log("Transfer", $"CustomerId: {customer.Id}, Amount: {amount}, From: {from.Id}, To: {to.Id}");
    }

    private static Customer LoadCustomerWithAccountsAndTransactions(BankingSystemDbContext db, Guid customerId)
    {
        return db.Customers
            .Include(c => c.Accounts)
            .Include(c => c.Transactions)
            .FirstOrDefault(c => c.Id == customerId)
            ?? throw new InvalidOperationException("Customer not found.");
    }
}
