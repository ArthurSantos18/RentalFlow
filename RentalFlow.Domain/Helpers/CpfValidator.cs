namespace RentalFlow.Domain.Helpers;

public static class CpfValidator
{
    public static bool IsValid(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
        {
            return false;
        }

        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
        {
            return false;
        }

        if (cpf.Distinct().Count() == 1)
        {
            return false;
        }

        int[] firstMultiplicator = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] secondMultiplicator = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        string tempCpf = cpf[..9];
        int sum = 0;

        for (int i = 0; i < 9; i++)
        {
            sum += int.Parse(tempCpf[i].ToString()) * firstMultiplicator[i];
        }


        int remainder = sum % 11;
        int digit = remainder < 2 ? 0 : 11 - remainder;

        if (digit != int.Parse(cpf[9].ToString()))
        {
            return false;
        }

        tempCpf += digit;
        sum = 0;

        for (int i = 0; i < 10; i++)
            sum += int.Parse(tempCpf[i].ToString()) * secondMultiplicator[i];

        remainder = sum % 11;
        digit = remainder < 2 ? 0 : 11 - remainder;

        return remainder == int.Parse(cpf[10].ToString());
    }
}
