using BankingSystem.Core.Interfaces;
using BankingSystem.Core.Logging;
using BankingSystem.Core.Models;
using BankingSystem.WPF.Db;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.WPF.Services;

public sealed class EfCreditCardService : ICreditCardService
{
    private readonly DbContextOptions<BankingSystemDbContext> _dbOptions;
    private readonly ILoggerService _logger;

    public EfCreditCardService(DbContextOptions<BankingSystemDbContext> dbOptions, ILoggerService logger)
    {
        _dbOptions = dbOptions;
        _logger = logger;
    }

    public CreditCard Create(Guid customerId, int limit)
    {
        using var db = new BankingSystemDbContext(_dbOptions);

        var customer = db.Customers
            .Include(c => c.CreditCard)
            .FirstOrDefault(c => c.Id == customerId)
            ?? throw new InvalidOperationException("Customer not found.");

        if (customer.CreditCard is not null)
            throw new InvalidOperationException("Only one credit card is allowed per customer.");

        var card = new CreditCard(limit);
        customer.AddCreditCard(card);
        db.SaveChanges();

        _logger.Log("Create Credit Card", $"CustomerId: {customer.Id}, Limit: {limit}");
        return card;
    }

    public void UpdateLimit(Guid customerId, int newLimit)
    {
        using var db = new BankingSystemDbContext(_dbOptions);

        var customer = db.Customers
            .Include(c => c.CreditCard)
            .FirstOrDefault(c => c.Id == customerId)
            ?? throw new InvalidOperationException("Customer not found.");

        var card = customer.CreditCard ?? throw new InvalidOperationException("Customer has no credit card.");
        card.UpdateLimit(newLimit);
        db.SaveChanges();

        _logger.Log("Update Credit Card", $"CustomerId: {customer.Id}, Limit: {newLimit}");
    }
}
