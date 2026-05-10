using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using BankingSystem.Core.Enums;
using BankingSystem.Core.Interfaces;
using BankingSystem.Core.Models;
using BankingSystem.WPF.Commands;

namespace BankingSystem.WPF.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private static readonly Brush NormalMessageBrush =
        new SolidColorBrush(Color.FromRgb(30, 74, 98)); // matches former info tone (~ #1E4A62)

    private readonly ICustomerService _customerService;
    private readonly IAccountService _accountService;
    private readonly ICertificateService _certificateService;
    private readonly ICreditCardService _creditCardService;
    private readonly IReportingService _reportingService;
    private CustomerDisplayViewModel? _selectedCustomer;
    private string _message = "Ready";
    private Brush _messageForeground = NormalMessageBrush;
    private Guid _selectedAccountId;
    private Guid _transferToAccountId;
    private Guid _selectedCertificateId;
    private string _name = string.Empty;
    private int _age;
    private Gender _gender = Gender.Male;
    private string _address = string.Empty;
    private string _nationalId = string.Empty;

    public MainViewModel(
        ICustomerService customerService,
        IAccountService accountService,
        ICertificateService certificateService,
        ICreditCardService creditCardService,
        IReportingService reportingService)
    {
        _customerService = customerService;
        _accountService = accountService;
        _certificateService = certificateService;
        _creditCardService = creditCardService;
        _reportingService = reportingService;

        Customers = new ObservableCollection<CustomerDisplayViewModel>();
        CustomerTransactions = new ObservableCollection<string>();
        CustomerServiceActivities = new ObservableCollection<string>();

        ReloadCustomers();

        AddCustomerCommand = new RelayCommand(_ => AddCustomer());
        EditCustomerCommand = new RelayCommand(_ => EditCustomer(), _ => SelectedCustomer is not null);
        CloseCustomerCommand = new RelayCommand(_ => CloseCustomer(), _ => SelectedCustomer is not null);
        AddAccountCommand = new RelayCommand(_ => AddAccount(), _ => SelectedCustomer is not null);
        DepositCommand = new RelayCommand(_ => Deposit(), _ => SelectedCustomer is not null && SelectedAccountId != Guid.Empty);
        WithdrawCommand = new RelayCommand(_ => Withdraw(), _ => SelectedCustomer is not null && SelectedAccountId != Guid.Empty);
        TransferCommand = new RelayCommand(_ => Transfer(), _ => SelectedCustomer is not null && SelectedAccountId != Guid.Empty && TransferToAccountId != Guid.Empty);
        CreateCertificateCommand = new RelayCommand(_ => CreateCertificate(), _ => SelectedCustomer is not null);
        UpdateCertificateCommand = new RelayCommand(_ => UpdateCertificate(), _ => SelectedCustomer is not null && SelectedCertificateId != Guid.Empty);
        DeleteCertificateCommand = new RelayCommand(_ => DeleteCertificate(), _ => SelectedCustomer is not null && SelectedCertificateId != Guid.Empty);
        CreateCreditCardCommand = new RelayCommand(_ => CreateCreditCard(), _ => SelectedCustomer is not null);
        UpdateCreditCardCommand = new RelayCommand(_ => UpdateCreditCard(), _ => SelectedCustomer is not null);
        GenerateCustomerReportCommand = new RelayCommand(_ => GenerateCustomerReport(), _ => SelectedCustomer is not null);
        GenerateStatisticsCommand = new RelayCommand(_ => GenerateStatistics());
    }

    public ObservableCollection<CustomerDisplayViewModel> Customers { get; }
    public ObservableCollection<string> CustomerTransactions { get; }
    public ObservableCollection<string> CustomerServiceActivities { get; }

    public string Name
    {
        get => _name;
        set
        {
            if (_name == value) return;
            _name = value;
            OnPropertyChanged();
        }
    }

    public int Age
    {
        get => _age;
        set
        {
            if (_age == value) return;
            _age = value;
            OnPropertyChanged();
        }
    }

    public Gender Gender
    {
        get => _gender;
        set
        {
            if (_gender == value) return;
            _gender = value;
            OnPropertyChanged();
        }
    }

    public string Address
    {
        get => _address;
        set
        {
            if (_address == value) return;
            _address = value;
            OnPropertyChanged();
        }
    }

    public string NationalId
    {
        get => _nationalId;
        set
        {
            if (_nationalId == value) return;
            _nationalId = value;
            OnPropertyChanged();
        }
    }

    public AccountType AccountType { get; set; } = AccountType.Saving;
    public decimal InitialBalance { get; set; }

    public Guid SelectedAccountId
    {
        get => _selectedAccountId;
        set
        {
            if (_selectedAccountId == value) return;
            _selectedAccountId = value;
            OnPropertyChanged();
            RelayCommand.NotifyCanExecuteChanged();
        }
    }

    public Guid TransferToAccountId
    {
        get => _transferToAccountId;
        set
        {
            if (_transferToAccountId == value) return;
            _transferToAccountId = value;
            OnPropertyChanged();
            RelayCommand.NotifyCanExecuteChanged();
        }
    }

    public decimal Amount { get; set; }

    public CertificatePeriod CertificatePeriod { get; set; } = CertificatePeriod.OneYear;
    public decimal CertificatePrice { get; set; } = 1000;

    public Guid SelectedCertificateId
    {
        get => _selectedCertificateId;
        set
        {
            if (_selectedCertificateId == value) return;
            _selectedCertificateId = value;
            OnPropertyChanged();
            RelayCommand.NotifyCanExecuteChanged();
        }
    }

    public int CreditCardLimit { get; set; } = 50000;

    public string Message
    {
        get => _message;
        set
        {
            _message = value;
            OnPropertyChanged();
        }
    }

    public Brush MessageForeground
    {
        get => _messageForeground;
        set
        {
            _messageForeground = value;
            OnPropertyChanged();
        }
    }

    public CustomerDisplayViewModel? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            _selectedCustomer = value;
            OnPropertyChanged();
            LoadSelectedCustomerIntoTopFields(_selectedCustomer?.Customer);
            ResetAccountSelectionState();
            RefreshCustomerDetails();
        }
    }

    /// <summary>
    /// Clearing IDs when switching customers avoids stale account GUIDs disabling commands or misleading the UI.
    /// </summary>
    private void ResetAccountSelectionState()
    {
        _selectedAccountId = Guid.Empty;
        _transferToAccountId = Guid.Empty;
        _selectedCertificateId = Guid.Empty;
        OnPropertyChanged(nameof(SelectedAccountId));
        OnPropertyChanged(nameof(TransferToAccountId));
        OnPropertyChanged(nameof(SelectedCertificateId));
        RelayCommand.NotifyCanExecuteChanged();
    }

    public ICommand AddCustomerCommand { get; }
    public ICommand EditCustomerCommand { get; }
    public ICommand CloseCustomerCommand { get; }
    public ICommand AddAccountCommand { get; }
    public ICommand DepositCommand { get; }
    public ICommand WithdrawCommand { get; }
    public ICommand TransferCommand { get; }
    public ICommand CreateCertificateCommand { get; }
    public ICommand UpdateCertificateCommand { get; }
    public ICommand DeleteCertificateCommand { get; }
    public ICommand CreateCreditCardCommand { get; }
    public ICommand UpdateCreditCardCommand { get; }
    public ICommand GenerateCustomerReportCommand { get; }
    public ICommand GenerateStatisticsCommand { get; }

    private void AddCustomer()
    {
        ExecuteSafely(() =>
        {
            var customer = _customerService.Add(Name, Age, Gender, Address, NationalId);
            ReloadCustomers();
            SelectedCustomer = Customers.FirstOrDefault(x => x.Customer.Id == customer.Id);
            Message = "Customer added successfully.";
        });
    }

    private void EditCustomer()
    {
        ExecuteSafely(() =>
        {
            var customer = SelectedCustomer!.Customer;
            _customerService.Edit(customer.Id, Name, Age, Gender, Address, NationalId);
            ReloadCustomers();
            Message = "Customer updated successfully.";
        });
    }

    private void CloseCustomer()
    {
        ExecuteSafely(() =>
        {
            var customer = SelectedCustomer!.Customer;
            _customerService.Close(customer.Id);
            ReloadCustomers();
            Message = "Customer closed successfully.";
            OnPropertyChanged(nameof(SelectedCustomer));
        });
    }

    private void ReloadCustomers()
    {
        var selectedId = SelectedCustomer?.Customer.Id;
        Customers.Clear();
        foreach (var customer in _customerService.GetAll())
        {
            Customers.Add(new CustomerDisplayViewModel(customer));
        }

        if (selectedId is not null)
        {
            SelectedCustomer = Customers.FirstOrDefault(x => x.Customer.Id == selectedId.Value);
        }
    }

    private void LoadSelectedCustomerIntoTopFields(Customer? customer)
    {
        if (customer is null) return;

        // Populate the editable top bar when a customer is selected.
        Name = customer.Name;
        Age = customer.Age;
        Gender = customer.Gender;
        Address = customer.Address;
        NationalId = customer.NationalId;
    }

    private void AddAccount()
    {
        ExecuteSafely(() =>
        {
            var customer = SelectedCustomer!.Customer;
            var account = _accountService.AddAccount(customer.Id, AccountType, InitialBalance);
            SelectedAccountId = account.Id;
            Message = "Account created successfully.";
            RefreshCustomerDetails();
        });
    }

    private void Deposit()
    {
        ExecuteSafely(() =>
        {
            var customer = SelectedCustomer!.Customer;
            _accountService.Deposit(customer.Id, SelectedAccountId, Amount);
            Message = "Deposit completed.";
            RefreshCustomerDetails();
        });
    }

    private void Withdraw()
    {
        ExecuteSafely(() =>
        {
            var customer = SelectedCustomer!.Customer;
            _accountService.Withdraw(customer.Id, SelectedAccountId, Amount);
            Message = "Withdraw completed.";
            RefreshCustomerDetails();
        });
    }

    private void Transfer()
    {
        ExecuteSafely(() =>
        {
            var customer = SelectedCustomer!.Customer;
            _accountService.Transfer(customer.Id, SelectedAccountId, TransferToAccountId, Amount);
            Message = "Transfer completed.";
            RefreshCustomerDetails();
        });
    }

    private void CreateCertificate()
    {
        ExecuteSafely(() =>
        {
            var customer = SelectedCustomer!.Customer;
            var certificate = _certificateService.Create(customer.Id, CertificatePeriod, CertificatePrice);
            SelectedCertificateId = certificate.Id;
            Message = "Certificate created successfully.";
            RefreshCustomerDetails();
        });
    }

    private void UpdateCertificate()
    {
        ExecuteSafely(() =>
        {
            var customer = SelectedCustomer!.Customer;
            _certificateService.Update(customer.Id, SelectedCertificateId, CertificatePeriod, CertificatePrice);
            Message = "Certificate updated successfully.";
            RefreshCustomerDetails();
        });
    }

    private void DeleteCertificate()
    {
        ExecuteSafely(() =>
        {
            var customer = SelectedCustomer!.Customer;
            _certificateService.Delete(customer.Id, SelectedCertificateId);
            Message = "Certificate deleted successfully.";
            RefreshCustomerDetails();
        });
    }

    private void CreateCreditCard()
    {
        ExecuteSafely(() =>
        {
            var customer = SelectedCustomer!.Customer;
            _creditCardService.Create(customer.Id, CreditCardLimit);
            Message = "Credit card created successfully.";
            RefreshCustomerDetails();
        });
    }

    private void UpdateCreditCard()
    {
        ExecuteSafely(() =>
        {
            var customer = SelectedCustomer!.Customer;
            _creditCardService.UpdateLimit(customer.Id, CreditCardLimit);
            Message = "Credit card limit updated successfully.";
            RefreshCustomerDetails();
        });
    }

    private void GenerateCustomerReport()
    {
        ExecuteSafely(() =>
        {
            var customer = SelectedCustomer!.Customer;
            var report = _reportingService.GenerateCustomerReport(customer.Id);
            Message = $"Report => Name: {report.CustomerName}, Final Balance: {report.FinalBalance}";
            RefreshCustomerDetails();
        });
    }

    private void GenerateStatistics()
    {
        ExecuteSafely(() =>
        {
            var stats = _reportingService.GenerateBankStatistics();
            Message = $"Customers: {stats.TotalCustomers}, Assets: {stats.TotalAssets}, Certificates: {stats.TotalCertificates}, Cards: {stats.TotalCreditCards}";
        });
    }

    private void RefreshCustomerDetails()
    {
        CustomerTransactions.Clear();
        CustomerServiceActivities.Clear();

        if (SelectedCustomer is null)
        {
            RelayCommand.NotifyCanExecuteChanged();
            return;
        }

        var selectedId = SelectedCustomer.Customer.Id;
        Customer customer;
        try
        {
            customer = _customerService.GetById(selectedId);
        }
        catch
        {
            RelayCommand.NotifyCanExecuteChanged();
            return;
        }

        foreach (var transaction in customer.Transactions)
        {
            CustomerTransactions.Add($"{transaction.Action} - {transaction.Amount}");
        }

        foreach (var activity in customer.ServiceActivities)
        {
            CustomerServiceActivities.Add(activity);
        }

        if (customer.Accounts.Count > 0 && _selectedAccountId == Guid.Empty)
        {
            SelectedAccountId = customer.Accounts[0].Id;
        }

        if (customer.Accounts.Count > 1 && _transferToAccountId == Guid.Empty)
        {
            TransferToAccountId = customer.Accounts[1].Id;
        }

        if (customer.Certificates.Count > 0 && _selectedCertificateId == Guid.Empty)
        {
            SelectedCertificateId = customer.Certificates[0].Id;
        }

        RelayCommand.NotifyCanExecuteChanged();
    }

    private void ExecuteSafely(Action action)
    {
        try
        {
            action();
            MessageForeground = NormalMessageBrush;
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            MessageForeground = Brushes.Red;
        }
    }
}
