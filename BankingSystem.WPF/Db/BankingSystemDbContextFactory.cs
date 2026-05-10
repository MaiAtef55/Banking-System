using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BankingSystem.WPF.Db;

public sealed class BankingSystemDbContextFactory : IDesignTimeDbContextFactory<BankingSystemDbContext>
{
    public BankingSystemDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BankingSystemDbContext>();
        optionsBuilder.UseSqlServer("Server=DESKTOP-UKSBV08;Database=BankingSystemDb;Trusted_Connection=True;TrustServerCertificate=True");

        return new BankingSystemDbContext(optionsBuilder.Options);
    }
}
