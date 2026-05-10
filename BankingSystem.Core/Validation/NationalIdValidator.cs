namespace BankingSystem.Core.Validation;

/// <summary>National ID: exactly 14 digits (non-digits stripped). No other rules.</summary>
public static class NationalIdValidator
{
    public const int Length = 14;

    public static string NormalizeAndValidate(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
            throw new ArgumentException(ValidationMessages.MandatoryField);

        var digits = string.Concat(nationalId.Trim().Where(char.IsDigit));

        if (digits.Length != Length)
            throw new ArgumentException($"National ID must be {Length} digits.");

        return digits;
    }
}
