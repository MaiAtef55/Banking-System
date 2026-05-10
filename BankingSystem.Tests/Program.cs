using BankingSystem.Core.Enums;
using BankingSystem.Core.Interfaces;
using BankingSystem.Core.Models;
using BankingSystem.Core.Services;

namespace BankingSystem.Tests;

internal static class Program
{
    private static int _passed;
    private static int _failed;

    private static void Main()
    {
        RunTest("Deposit increases balance", Deposit_IncreasesBalance);
        RunTest("Withdraw decreases balance", Withdraw_DecreasesBalance);
        RunTest("Transfer moves amount", Transfer_MovesAmount);
        RunTest("Certificate validation works", Certificate_Validation);
        RunTest("Credit card rules are enforced", CreditCard_Rules);
        RunTest("National ID must be 14 digits", NationalId_Length14);
        RunTest("National ID cannot duplicate", NationalId_NoDuplicate);

        Console.WriteLine($"Passed: {_passed}, Failed: {_failed}");
        Environment.ExitCode = _failed == 0 ? 0 : 1;
    }

    private static void Deposit_IncreasesBalance()
    {
        var service = CreateService();
        var customer = service.AddCustomer("T1", 30, Gender.Male, "Addr", "29801015567890");
        var account = service.AddAccount(customer.Id, AccountType.Saving, 1000);

        service.Deposit(customer.Id, account.Id, 500);

        AssertEqual(1500m, account.Balance, "Deposit failed");
    }

    private static void Withdraw_DecreasesBalance()
    {
        var service = CreateService();
        var customer = service.AddCustomer("T2", 28, Gender.Female, "Addr", "29702025567891");
        var account = service.AddAccount(customer.Id, AccountType.Salary, 2000);

        service.Withdraw(customer.Id, account.Id, 700);

        AssertEqual(1300m, account.Balance, "Withdraw failed");
    }

    private static void Transfer_MovesAmount()
    {
        var service = CreateService();
        var customer = service.AddCustomer("T3", 26, Gender.Female, "Addr", "29603035567892");
        var from = service.AddAccount(customer.Id, AccountType.Saving, 3000);
        var to = service.AddAccount(customer.Id, AccountType.Salary, 1000);

        service.Transfer(customer.Id, from.Id, to.Id, 500);

        AssertEqual(2500m, from.Balance, "Transfer sender balance failed");
        AssertEqual(1500m, to.Balance, "Transfer receiver balance failed");
    }

    private static void Certificate_Validation()
    {
        var service = CreateService();
        var customer = service.AddCustomer("T4", 33, Gender.Male, "Addr", "29504045567893");

        var certificate = service.CreateCertificate(customer.Id, CertificatePeriod.FiveYears, 5000);
        AssertEqual(0.20m, certificate.InterestRate, "Interest rate failed");

        var failed = false;
        try
        {
            service.CreateCertificate(customer.Id, CertificatePeriod.OneYear, 1500);
        }
        catch (ArgumentException)
        {
            failed = true;
        }

        if (!failed) throw new Exception("Certificate validation should reject non-multiple of 1000.");
    }

    private static void CreditCard_Rules()
    {
        var service = CreateService();
        var customer = service.AddCustomer("T5", 35, Gender.Male, "Addr", "29405055567894");

        var card = service.CreateCreditCard(customer.Id, 100000);
        AssertEqual(100000, card.CashLimit, "Credit card create failed");

        service.UpdateCreditCardLimit(customer.Id, 150000);
        AssertEqual(150000, card.CashLimit, "Credit card update failed");

        var secondCardFailed = false;
        try
        {
            service.CreateCreditCard(customer.Id, 120000);
        }
        catch (InvalidOperationException)
        {
            secondCardFailed = true;
        }

        if (!secondCardFailed) throw new Exception("Second credit card must be rejected.");
    }

    private static void NationalId_Length14()
    {
        var service = CreateService();
        var rejected = false;
        try
        {
            service.AddCustomer("X", 20, Gender.Male, "Addr", "12345");
        }
        catch (ArgumentException)
        {
            rejected = true;
        }

        if (!rejected)
            throw new Exception("National ID with fewer than 14 digits must be rejected.");

        var ok = service.AddCustomer("Y", 21, Gender.Female, "Addr", " 29306065567895 ");
        AssertEqual("29306065567895", ok.NationalId, "Leading/trailing spaces: digits only stored.");
    }

    private static void NationalId_NoDuplicate()
    {
        var service = CreateService();
        service.AddCustomer("A", 30, Gender.Male, "Addr", "29207075567896");
        var dupRejected = false;
        try
        {
            service.AddCustomer("B", 31, Gender.Female, "Addr", "29207075567896");
        }
        catch (InvalidOperationException)
        {
            dupRejected = true;
        }

        if (!dupRejected)
            throw new Exception("Duplicate national ID must be rejected.");
    }

    private static BankService CreateService()
    {
        return new BankService(new FakeLogger());
    }

    private static void RunTest(string name, Action test)
    {
        try
        {
            test();
            _passed++;
            Console.WriteLine($"[PASS] {name}");
        }
        catch (Exception ex)
        {
            _failed++;
            Console.WriteLine($"[FAIL] {name} => {ex.Message}");
        }
    }

    private static void AssertEqual<T>(T expected, T actual, string message)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new Exception($"{message}. Expected: {expected}, Actual: {actual}");
    }

    private sealed class FakeLogger : ILoggerService
    {
        public void Log(string activity, string details)
        {
        }
    }
}
