using BankingSystem.Core.Enums;

namespace BankingSystem.Core.Models;

public sealed class SalaryAccount : Account
{
    private SalaryAccount() : base(AccountType.Salary, 0)
    {
    }

    public SalaryAccount(decimal initialBalance = 0) : base(AccountType.Salary, initialBalance)
    {
    }
}
