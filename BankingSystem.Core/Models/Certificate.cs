using BankingSystem.Core.Enums;

namespace BankingSystem.Core.Models;

public sealed class Certificate
{
    private Certificate()
    {
    }

    public Certificate(CertificatePeriod period, decimal price)
    {
        if (price < 1000 || price % 1000 != 0)
            throw new ArgumentException("Certificate price must be at least 1000 and multiple of 1000.");

        Id = Guid.NewGuid();
        Period = period;
        Price = price;
    }

    public Guid Id { get; private set; }
    public CertificatePeriod Period { get; private set; }
    public decimal Price { get; private set; }

    public decimal InterestRate => Period switch
    {
        CertificatePeriod.OneYear => 0.10m,
        CertificatePeriod.ThreeYears => 0.15m,
        CertificatePeriod.FiveYears => 0.20m,
        _ => 0m
    };

    public void Update(CertificatePeriod period, decimal price)
    {
        if (price < 1000 || price % 1000 != 0)
            throw new ArgumentException("Certificate price must be at least 1000 and multiple of 1000.");

        Period = period;
        Price = price;
    }
}
