using BankingSystem.Core.Enums;
using BankingSystem.Core.Models;
using BankingSystem.Core.Services;

namespace BankingSystem.Core.Interfaces;

public interface IBankService
{
    IReadOnlyCollection<Customer> Customers { get; }

    Customer AddCustomer(string name, int age, Gender gender, string address, string nationalId);
    void EditCustomer(Guid customerId, string name, int age, Gender gender, string address, string nationalId);
    void CloseCustomer(Guid customerId);

    Account AddAccount(Guid customerId, AccountType type, decimal initialBalance = 0);
    void Deposit(Guid customerId, Guid accountId, decimal amount);
    void Withdraw(Guid customerId, Guid accountId, decimal amount);
    void Transfer(Guid customerId, Guid fromAccountId, Guid toAccountId, decimal amount);

    Certificate CreateCertificate(Guid customerId, CertificatePeriod period, decimal price);
    void UpdateCertificate(Guid customerId, Guid certificateId, CertificatePeriod period, decimal price);
    void DeleteCertificate(Guid customerId, Guid certificateId);

    CreditCard CreateCreditCard(Guid customerId, int limit);
    void UpdateCreditCardLimit(Guid customerId, int newLimit);

    CustomerReport GenerateCustomerReport(Guid customerId);
    BankStatistics GenerateBankStatistics();
}
