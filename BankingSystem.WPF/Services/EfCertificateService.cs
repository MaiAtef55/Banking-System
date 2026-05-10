using BankingSystem.Core.Enums;
using BankingSystem.Core.Interfaces;
using BankingSystem.Core.Logging;
using BankingSystem.Core.Models;
using BankingSystem.WPF.Db;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.WPF.Services;

public sealed class EfCertificateService : ICertificateService
{
    private readonly DbContextOptions<BankingSystemDbContext> _dbOptions;
    private readonly ILoggerService _logger;

    public EfCertificateService(DbContextOptions<BankingSystemDbContext> dbOptions, ILoggerService logger)
    {
        _dbOptions = dbOptions;
        _logger = logger;
    }

    public Certificate Create(Guid customerId, CertificatePeriod period, decimal price)
    {
        using var db = new BankingSystemDbContext(_dbOptions);

        var customer = db.Customers
            .Include(c => c.Certificates)
            .FirstOrDefault(c => c.Id == customerId)
            ?? throw new InvalidOperationException("Customer not found.");

        var certificate = new Certificate(period, price);
        customer.Certificates.Add(certificate);
        db.SaveChanges();

        _logger.Log("Create Certificate", $"CustomerId: {customer.Id}, Period: {period}, Price: {price}");
        return certificate;
    }

    public void Update(Guid customerId, Guid certificateId, CertificatePeriod period, decimal price)
    {
        using var db = new BankingSystemDbContext(_dbOptions);

        var customer = db.Customers
            .Include(c => c.Certificates)
            .FirstOrDefault(c => c.Id == customerId)
            ?? throw new InvalidOperationException("Customer not found.");

        var certificate = customer.Certificates.FirstOrDefault(c => c.Id == certificateId)
            ?? throw new InvalidOperationException("Certificate not found.");

        certificate.Update(period, price);
        db.SaveChanges();
    }

    public void Delete(Guid customerId, Guid certificateId)
    {
        using var db = new BankingSystemDbContext(_dbOptions);

        var customer = db.Customers
            .Include(c => c.Certificates)
            .FirstOrDefault(c => c.Id == customerId)
            ?? throw new InvalidOperationException("Customer not found.");

        var certificate = customer.Certificates.FirstOrDefault(c => c.Id == certificateId)
            ?? throw new InvalidOperationException("Certificate not found.");

        customer.Certificates.Remove(certificate);
        db.SaveChanges();
    }
}
