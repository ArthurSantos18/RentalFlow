using System.Text.RegularExpressions;

namespace RentalFlow.Domain.Helpers;

public static partial class CpfValidator
{
    [GeneratedRegex(@"^\d{11}$|^\d{3}\.\d{3}\.\d{3}-\d{2}$")]
    private static partial Regex CpfFormatRegex();

    public static bool IsValid(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf) || !CpfFormatRegex().IsMatch(cpf))
        {
            return false;
        }

        var numericCpf = Normalize(cpf);

        if (numericCpf.Distinct().Count() == 1)
        {
            return false;
        }

        var firstCheckDigit = CalculateCheckDigit(numericCpf, 9);
        if (firstCheckDigit != (numericCpf[9] - '0'))
        {
            return false;
        }

        var secondCheckDigit = CalculateCheckDigit(numericCpf, 10);
        if (secondCheckDigit != (numericCpf[10] - '0'))
        {
            return false;
        }

        return true;
    }

    private static int CalculateCheckDigit(string cpf, int length)
    {
        var sum = 0;

        for (int i = 0; i < length; i++)
        {
            var weight = length + 1 - i;
            sum += (cpf[i] - '0') * weight;
        }

        var remainder = sum % 11;

        return remainder < 2 ? 0 : 11 - remainder;
    }

    public static string Normalize(string cpf)
    {
        return new string([.. cpf.Where(char.IsDigit)]);
    }
}
