using BankingSystem.Core.Enums;
using BankingSystem.Core.Models;

namespace BankingSystem.Core.Interfaces;

public interface IAccountService
{
    Account AddAccount(Guid customerId, AccountType type, decimal initialBalance = 0);
    void Deposit(Guid customerId, Guid accountId, decimal amount);
    void Withdraw(Guid customerId, Guid accountId, decimal amount);
    void Transfer(Guid customerId, Guid fromAccountId, Guid toAccountId, decimal amount);
}
