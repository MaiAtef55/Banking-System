using BankingSystem.Core.Enums;

namespace BankingSystem.Core.Models;

public sealed class SavingAccount : Account
{
    private SavingAccount() : base(AccountType.Saving, 0)
    {
    }

    public SavingAccount(decimal initialBalance = 0) : base(AccountType.Saving, initialBalance)
    {
    }
}
