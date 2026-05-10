using BankingSystem.Core.Models;

namespace BankingSystem.WPF.ViewModels;

public sealed class CustomerDisplayViewModel
{
    public CustomerDisplayViewModel(Customer customer)
    {
        Customer = customer;
    }

    public Customer Customer { get; }
    public string Name => Customer.Name;
    public string NationalId => Customer.NationalId;
    public bool IsClosed => Customer.IsClosed;
}
