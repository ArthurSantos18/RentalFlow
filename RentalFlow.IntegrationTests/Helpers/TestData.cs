namespace RentalFlow.IntegrationTests.Helpers;

public static class TestData
{
    public static string GenerateCpf()
    {
        var random = new Random(Guid.NewGuid().GetHashCode());
        var digits = new int[9];

        for (var i = 0; i < 9; i++)
        {
            digits[i] = random.Next(10);
        }

        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += digits[i] * (10 - i);
        }

        var firstDigit = 11 - (sum % 11);
        if (firstDigit >= 10) firstDigit = 0;

        sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += digits[i] * (11 - i);
        }
        sum += firstDigit * 2;

        var secondDigit = 11 - (sum % 11);
        if (secondDigit >= 10) secondDigit = 0;

        return string.Concat(digits) + firstDigit + secondDigit;
    }

    public static string GenerateEmail(string? prefix = null)
    {
        prefix ??= "test";
        return $"{prefix}-{Guid.NewGuid():N}@example.com";
    }

    public static string GeneratePhone()
    {
        var random = new Random(Guid.NewGuid().GetHashCode());
        return $"11{random.Next(900000000, 999999999)}";
    }

    public static Address GenerateAddress(string? city = null, string? state = null)
    {
        var random = new Random(Guid.NewGuid().GetHashCode());

        return new Address(
            street: $"Rua Teste {random.Next(1, 9999)}",
            number: random.Next(1, 9999).ToString(),
            complement: "Apto 101",
            neighborhood: "Centro",
            city: city ?? "São Paulo",
            state: state ?? "SP",
            zipCode: random.Next(10000000, 99999999).ToString()
        );
    }
}