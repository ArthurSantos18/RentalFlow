namespace RentalFlow.Domain.Helpers;

public static partial class PasswordValidator
{
    public const int MinimumLength = 8;

    [GeneratedRegex(@"[A-Z]")]
    private static partial Regex UppercaseRegex();

    [GeneratedRegex(@"[a-z]")]
    private static partial Regex LowercaseRegex();

    [GeneratedRegex(@"[0-9]")]
    private static partial Regex NumberRegex();

    [GeneratedRegex(@"[^a-zA-Z0-9]")]
    private static partial Regex SpecialCharRegex();

    public static bool IsValid(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        if (password.Length < MinimumLength)
        {
            return false;
        }

        if (!UppercaseRegex().IsMatch(password))
        {
            return false;
        }

        if (!LowercaseRegex().IsMatch(password))
        {
            return false;
        }

        if (!NumberRegex().IsMatch(password))
        {
            return false;
        }

        if (!SpecialCharRegex().IsMatch(password))
        {
            return false;
        }

        return true;
    }

    public static bool HasMinimumLength(string? password)
        => !string.IsNullOrWhiteSpace(password) && password.Length >= MinimumLength;

    public static bool HasUppercase(string? password)
        => !string.IsNullOrWhiteSpace(password) && UppercaseRegex().IsMatch(password);

    public static bool HasLowercase(string? password)
        => !string.IsNullOrWhiteSpace(password) && LowercaseRegex().IsMatch(password);

    public static bool HasNumber(string? password)
        => !string.IsNullOrWhiteSpace(password) && NumberRegex().IsMatch(password);

    public static bool HasSpecialCharacter(string? password)
        => !string.IsNullOrWhiteSpace(password) && SpecialCharRegex().IsMatch(password);
}
