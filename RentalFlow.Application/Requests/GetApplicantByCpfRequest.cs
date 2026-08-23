namespace RentalFlow.Application.Requests;

public record GetApplicantByCpfRequest
{
    public string Cpf { get; init; } = string.Empty;
};
