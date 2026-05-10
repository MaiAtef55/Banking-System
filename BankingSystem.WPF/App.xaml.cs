using System.Windows;
using BankingSystem.Core.Interfaces;
using BankingSystem.Core.Logging;
using BankingSystem.WPF.Db;
using BankingSystem.WPF.Services;
using BankingSystem.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.WPF;

public partial class App : Application
{
    private const string ConnectionString = "Server=DESKTOP-UKSBV08;Database=BankingSystemDb;Trusted_Connection=True;TrustServerCertificate=True";

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var dbOptions = new DbContextOptionsBuilder<BankingSystemDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using (var dbContext = new BankingSystemDbContext(dbOptions))
        {
            dbContext.Database.Migrate();
        }

        ILoggerService logger = new TextFileLogger();
        var customerService = new EfCustomerService(dbOptions);
        var accountService = new EfAccountService(dbOptions, logger);
        var certificateService = new EfCertificateService(dbOptions, logger);
        var creditCardService = new EfCreditCardService(dbOptions, logger);
        var reportingService = new EfReportingService(dbOptions);

        var mainWindow = new MainWindow
        {
            DataContext = new MainViewModel(
                customerService,
                accountService,
                certificateService,
                creditCardService,
                reportingService)
        };

        mainWindow.Show();
    }
}
