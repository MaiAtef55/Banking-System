using BankingSystem.Core.Interfaces;
using BankingSystem.Core.Services;
using BankingSystem.WPF.Db;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.WPF.Services;

public sealed class EfReportingService : IReportingService
{
    private readonly DbContextOptions<BankingSystemDbContext> _dbOptions;

    public EfReportingService(DbContextOptions<BankingSystemDbContext> dbOptions)
    {
        _dbOptions = dbOptions;
    }

    public CustomerReport GenerateCustomerReport(Guid customerId)
    {
        using var db = new BankingSystemDbContext(_dbOptions);

        var customer = db.Customers
            .Include(c => c.Accounts)
            .Include(c => c.Transactions)
            .FirstOrDefault(c => c.Id == customerId)
            ?? throw new InvalidOperationException("Customer not found.");

        return new CustomerReport
        {
            CustomerName = customer.Name,
            FinalBalance = customer.TotalAmount,
            Transactions = customer.Transactions
                .Select(x => $"{x.OccurredAt:yyyy-MM-dd HH:mm} - {x.Action} - {x.Amount}")
                .ToList(),
            ServiceActivities = Array.Empty<string>()
        };
    }

    public BankStatistics GenerateBankStatistics()
    {
        using var db = new BankingSystemDbContext(_dbOptions);

        return new BankStatistics
        {
            TotalCustomers = db.Customers.Count(),
            TotalAssets = db.Customers.Sum(c => (decimal?)c.TotalAmount) ?? 0m,
            TotalCertificates = db.Certificates.Count(),
            TotalCreditCards = db.CreditCards.Count()
        };
    }
}
