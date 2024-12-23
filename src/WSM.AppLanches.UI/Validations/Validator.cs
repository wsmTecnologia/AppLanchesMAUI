using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WSM.AppLanches.UI.Validations
{
    public class Validator : IValidator
    {
        public string NomeErro { get; set; } = "";
        public string EmailErro { get; set; } = "";
        public string TelefoneErro { get; set; } = "";
        public string SenhaErro { get; set; } = "";

        private const string NomeVazioErroMsg = "Por favor, informe o seu nome.";
        private const string NomeInvalidoErroMsg = "Por favor, informe um nome válido.";
        private const string EmailVazioErroMsg = "Por favor, informe o seu email.";
        private const string EmailInvalidoErroMsg = "Por favor, informe um email válido.";
        private const string TelefoneVazioErroMsg = "Por favor, informe o seu telfone.";
        private const string TelefoneInvalidoErroMsg = "Por favor, informe telefone válido.";
        private const string SenhaVazioErroMsg = "Por favor, informe o sua senha.";
        private const string SenhaInvalidoErroMsg = "A senha deve conter pelo menos 8 caractres, incluindo letras e números";
        public Task<bool> Validar(string nome, string email, string telefone, string senha)
        {
            var isNomeValido = ValidarNome(nome);
            var isEmailValido = ValidarEmail(email);
            var isTelValido = ValidarTel(telefone);
            var isSenhaValido = ValidarSenha(senha);

            return Task.FromResult(isNomeValido &&  isEmailValido && isTelValido && isSenhaValido);
        }

        private bool ValidarNome(string nome)
        {
            if (string.IsNullOrEmpty(nome)) { NomeErro = NomeVazioErroMsg; return false; }
            if (nome.Length < 3) { NomeErro = NomeInvalidoErroMsg; return false; }

            NomeErro = "";
            return true;
        }

        private bool ValidarEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) { EmailErro = EmailVazioErroMsg; return false; }
            if (!Regex.IsMatch(email,@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$") { EmailErro = EmailInvalidoErroMsg; return false; }

            EmailErro = "";
            return true;
        }

        private bool ValidarTel(string tel)
        {
            if (string.IsNullOrEmpty(tel)) { TelefoneErro = TelefoneVazioErroMsg; return false; }
            if (tel.Length < 12) { TelefoneErro = TelefoneInvalidoErroMsg; return false; }

            TelefoneErro = "";
            return true;
        }

        private bool ValidarSenha(string senha)
        {
            if (string.IsNullOrEmpty(senha)) { SenhaErro = SenhaVazioErroMsg; return false; }
            if (senha.Length < 8 && !Regex.IsMatch(senha, @"[a-zA-Z]") || !Regex.IsMatch(senha, @"\d")) { SenhaErro = SenhaInvalidoErroMsg; return false; }

            SenhaErro = "";
            return true;
        }
    }
}
