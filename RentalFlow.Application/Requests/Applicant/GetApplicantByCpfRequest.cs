namespace RentalFlow.Application.Requests.Applicant;

public record GetApplicantByCpfRequest
{
    public string Cpf { get; init; } = string.Empty;
};
