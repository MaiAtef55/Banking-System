using BankingSystem.Core.Enums;
using BankingSystem.Core.Interfaces;
using BankingSystem.Core.Models;
using BankingSystem.Core.Validation;
using BankingSystem.WPF.Db;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.WPF.Services;

public sealed class EfCustomerService : ICustomerService
{
    private readonly DbContextOptions<BankingSystemDbContext> _dbOptions;

    public EfCustomerService(DbContextOptions<BankingSystemDbContext> dbOptions)
    {
        _dbOptions = dbOptions;
    }

    public IReadOnlyCollection<Customer> GetAll()
    {
        using var db = new BankingSystemDbContext(_dbOptions);
        return db.Customers
            .Include(c => c.Accounts)
            .Include(c => c.Certificates)
            .Include(c => c.CreditCard)
            .Include(c => c.Transactions)
            .OrderBy(c => c.Name)
            .ToList();
    }

    public Customer GetById(Guid customerId)
    {
        using var db = new BankingSystemDbContext(_dbOptions);
        return db.Customers
            .Include(c => c.Accounts)
            .Include(c => c.Certificates)
            .Include(c => c.CreditCard)
            .Include(c => c.Transactions)
            .FirstOrDefault(c => c.Id == customerId)
            ?? throw new InvalidOperationException("Customer not found.");
    }

    public Customer Add(string name, int age, Gender gender, string address, string nationalId)
    {
        using var db = new BankingSystemDbContext(_dbOptions);

        var normalizedNationalId = NationalIdValidator.NormalizeAndValidate(nationalId);
        if (db.Customers.Any(c => c.NationalId == normalizedNationalId))
            throw new InvalidOperationException("National ID already exists.");

        var customer = new Customer(name, age, gender, address, normalizedNationalId);
        db.Customers.Add(customer);
        db.SaveChanges();
        return customer;
    }

    public void Edit(Guid customerId, string name, int age, Gender gender, string address, string nationalId)
    {
        using var db = new BankingSystemDbContext(_dbOptions);

        var customer = db.Customers.FirstOrDefault(x => x.Id == customerId)
            ?? throw new InvalidOperationException("Customer not found.");

        var normalizedNationalId = NationalIdValidator.NormalizeAndValidate(nationalId);
        if (db.Customers.Any(c => c.Id != customerId && c.NationalId == normalizedNationalId))
            throw new InvalidOperationException("National ID already exists.");

        customer.Update(name, age, gender, address, normalizedNationalId);
        db.SaveChanges();
    }

    public void Close(Guid customerId)
    {
        using var db = new BankingSystemDbContext(_dbOptions);

        var customer = db.Customers.FirstOrDefault(x => x.Id == customerId)
            ?? throw new InvalidOperationException("Customer not found.");

        customer.Close();
        db.SaveChanges();
    }
}
