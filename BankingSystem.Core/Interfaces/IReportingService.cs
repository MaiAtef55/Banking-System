using BankingSystem.Core.Services;

namespace BankingSystem.Core.Interfaces;

public interface IReportingService
{
    CustomerReport GenerateCustomerReport(Guid customerId);
    BankStatistics GenerateBankStatistics();
}
