using BankingSystem.Core.Models;

namespace BankingSystem.Core.Interfaces;

public interface ICreditCardService
{
    CreditCard Create(Guid customerId, int limit);
    void UpdateLimit(Guid customerId, int newLimit);
}
