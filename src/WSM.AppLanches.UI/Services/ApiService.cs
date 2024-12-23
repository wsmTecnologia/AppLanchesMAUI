using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using WSM.AppLanches.UI.Models;

namespace WSM.AppLanches.UI.Services
{
    public class ApiService
    {
        private readonly HttpClient? _httpClient;
        private readonly string _baseUrl = "https://hp5mlv05-7265.brs.devtunnels.ms/";
        private readonly ILogger<ApiService>? _logger;

        JsonSerializerOptions _serializerOptions;

        public ApiService(HttpClient? httpClient, ILogger<ApiService>? logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<ApiResponse<bool>> Login(string username, string password)
        {
            try
            {
                var login = new Login()
                {
                    Email = username,
                    Senha = password,                    
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8,"application/json");

                var response = await PostResquest("api/Usuarios/Register", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error ao enviar requisição HTTP: { response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP:{response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Token>(jsonResult);

                Preferences.Set("acesstoken", result!.AccessToken);
                Preferences.Set("usuarioid", (int)result.UsuarioId!);
                Preferences.Set("usuarionome", result.UsuarioNome);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error login: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };

            }
        }

        public async Task<ApiResponse<bool>> RegistrarUSuario(string username, string password, string tel, string nome)
        {
            try
            {
                var login = new Register()
                {
                    Email = username,
                    Nome = nome,
                    Telefone = tel,
                    Senha = password
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostResquest("api/Usuarios/Login", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP:{response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error login: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };

            }
        }

        private async Task<HttpResponseMessage> PostResquest(string uri, HttpContent content)
        {
            var enderecoUrl = _baseUrl + uri;
            try
            {
                var result = await _httpClient.PostAsync(enderecoUrl, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ao enviar requisiçao POST para {uri}: {ex.Message}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }

    }
}
