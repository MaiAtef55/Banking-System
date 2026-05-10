using BankingSystem.Core.Enums;
using BankingSystem.Core.Models;

namespace BankingSystem.Core.Interfaces;

public interface ICustomerService
{
    IReadOnlyCollection<Customer> GetAll();
    Customer GetById(Guid customerId);

    Customer Add(string name, int age, Gender gender, string address, string nationalId);
    void Edit(Guid customerId, string name, int age, Gender gender, string address, string nationalId);
    void Close(Guid customerId);
}
