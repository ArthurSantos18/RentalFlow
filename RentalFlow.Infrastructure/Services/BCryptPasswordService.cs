using BCrypt.Net;
using RentalFlow.Application.Interfaces.Services;

namespace RentalFlow.Infrastructure.Services;

public sealed class BCryptPasswordService : IPasswordService
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        catch (SaltParseException)
        {
            return false;
        }
    }
}
