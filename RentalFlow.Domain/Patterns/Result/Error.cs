namespace RentalFlow.Domain.Patterns.Result;

public record Error(int Code, string Message)
{
    public static readonly Error None = new(0, string.Empty);
}
