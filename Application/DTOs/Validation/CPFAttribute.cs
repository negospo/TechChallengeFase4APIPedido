using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Validation
{
    public class CPFAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var cpf = Convert.ToString(value);

            // Remove pontos, traços e espaços.
            cpf = cpf.Replace(".", "").Replace("-", "").Trim();

            if (cpf.Length != 11)
                return false;

            if (!long.TryParse(cpf, out _))
                return false;

            if (SequenciaRepetida(cpf))
                return false;

            var tempCpf = cpf.Substring(0, 9);
            tempCpf += CalcularDigito(tempCpf);
            tempCpf += CalcularDigito(tempCpf);

            return cpf.EndsWith(tempCpf.Substring(9, 2));
        }

        private bool SequenciaRepetida(string sequencia)
        {
            string[] invalidos =
            {
            "00000000000", "11111111111", "22222222222", "33333333333",
            "44444444444", "55555555555", "66666666666", "77777777777",
            "88888888888", "99999999999"
        };
            return Array.Exists(invalidos, item => item == sequencia);
        }

        private int CalcularDigito(string tempCpf)
        {
            int soma = 0;
            for (int i = 0; i < tempCpf.Length; i++)
            {
                soma += (tempCpf.Length + 1 - i) * Convert.ToInt32(tempCpf[i].ToString());
            }
            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }
    }
}
