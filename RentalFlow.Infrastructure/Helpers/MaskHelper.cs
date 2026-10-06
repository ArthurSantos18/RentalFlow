namespace RentalFlow.Infrastructure.Helpers;

public static class MaskHelper
{
    public static string? Mask(string fieldName, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return fieldName switch
        {
            "Cpf" => MaskCpf(value),
            "Phone" => MaskPhone(value),
            "MonthlyIncome" => "*****",
            _ => value
        };
    }

    private static string MaskCpf(string cpf)
    {
        var digits = cpf.Replace(".", "").Replace("-", "");

        return digits.Length == 11 ? $"***.***.***-{digits[^2..]}" : "***";
    }

    private static string MaskPhone(string phone)
    {
        var digits = new string([.. phone.Where(char.IsDigit)]);

        return digits.Length >= 4 ? $"****-{digits[^4..]}" : "****";
    }
}