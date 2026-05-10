using BankingSystem.Core.Enums;
using BankingSystem.Core.Models;

namespace BankingSystem.Core.Interfaces;

public interface ICertificateService
{
    Certificate Create(Guid customerId, CertificatePeriod period, decimal price);
    void Update(Guid customerId, Guid certificateId, CertificatePeriod period, decimal price);
    void Delete(Guid customerId, Guid certificateId);
}
