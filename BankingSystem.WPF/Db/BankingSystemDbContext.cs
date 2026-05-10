using BankingSystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.WPF.Db;

public sealed class BankingSystemDbContext : DbContext
{
    public BankingSystemDbContext(DbContextOptions<BankingSystemDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<SavingAccount> SavingAccounts => Set<SavingAccount>();
    public DbSet<SalaryAccount> SalaryAccounts => Set<SalaryAccount>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<CreditCard> CreditCards => Set<CreditCard>();
    public DbSet<TransactionRecord> TransactionRecords => Set<TransactionRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.Address).IsRequired();
            entity.Property(x => x.NationalId).IsRequired();
            entity.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            entity.HasIndex(x => x.NationalId).IsUnique();
            entity.Ignore(x => x.ServiceActivities);

            entity.HasMany(x => x.Accounts)
                  .WithOne()
                  .HasForeignKey("CustomerId")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Certificates)
                  .WithOne()
                  .HasForeignKey("CustomerId")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Transactions)
                  .WithOne()
                  .HasForeignKey("CustomerId")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.CreditCard)
                  .WithOne()
                  .HasForeignKey<CreditCard>("CustomerId")
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Balance).HasColumnType("decimal(18,2)");
            entity.UseTptMappingStrategy();
            entity.ToTable("Accounts");
        });

        modelBuilder.Entity<SavingAccount>().ToTable("SavingAccounts");
        modelBuilder.Entity<SalaryAccount>().ToTable("SalaryAccounts");

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Price).HasColumnType("decimal(18,2)");
            entity.Ignore(x => x.InterestRate);
        });

        modelBuilder.Entity<CreditCard>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.HasIndex("CustomerId").IsUnique();
        });

        modelBuilder.Entity<TransactionRecord>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Action).IsRequired();
            entity.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        });
    }
}
