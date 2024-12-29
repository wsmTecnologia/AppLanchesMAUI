using Microsoft.Extensions.Logging;
using System.Reflection.Metadata;
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

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

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

                var response = await PostResquest("api/Usuarios/Register", content);
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

        public async Task<(List<Categoria>? Categorias, string ErrorMessage)> GetCategorias()
        {
            return await GetAsync<List<Categoria>>("api/categorias");
        }

        public async Task<(List<Produto>? Produtos, string ErrorMessage)> GetProduto(string tipoProduto, string categoriaId)
        {
            string endPoint = $"api/Produtos?tipoProduto={tipoProduto}&categoriaId={categoriaId}";
            return await GetAsync<List<Produto>>(endPoint);
        }

        public async Task<(Produto? ProdutoDetalhe, string ErrorMessage)> GetProdutoDetalhe(int prodId)
        {
            string endpoint = $"api/produtos/{prodId}";
            return await GetAsync<Produto>(endpoint);
        }

        public async Task<ApiResponse<bool>> AdicionaItemNoCarrinho(CarrinhoCompra carrinhoCompra)
        {
            try
            {
                var json = JsonSerializer.Serialize(carrinhoCompra, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostResquest("api/ItensCarrinhoCompra", content);
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
                _logger.LogError($"Error ao adicionar item no carrinho: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<(List<CarrinhoCompraItem>? carrinhoCompraItems, string? ErrorMessage)> GetItensCarrinhoCompra(int usuarioId)
        {
            var endpoint = $"api/ItensCarrinhoCompra/{usuarioId}";
            return await GetAsync<List<CarrinhoCompraItem>>(endpoint);
        }

        public async Task<(bool Data, string? ErrorMessage)> AtualizaQuantidadeItemCarrinho(int produtoId, string acao)
        {
            try
            {
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var response = await PostResquest($"api/ItensCarrinhoCompra?produtoId={produtoId}&acao={acao}", content);
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generealErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
                    _logger.LogError(generealErrorMessage);
                    return (default, generealErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Erro na requisição HTTP: {ex.Message}";
                _logger.LogError(ex.Message);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erro inesperado: {ex.Message}";
                _logger.LogError(ex.Message);
                return (default, errorMessage);
            }
        }

        public async Task<ApiResponse<bool>> ConfirmarPedido(Pedido pedido)
        {
            try
            {
                var json = JsonSerializer.Serialize(pedido, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostResquest("api/Pedidos", content);
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
                _logger.LogError($"Error ao confirmar pedido: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        private async Task<(T? Data, string ErrorMessage)> GetAsync<T>(string endpoint)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                    return (data ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generealErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
                    _logger.LogError(generealErrorMessage);
                    return (default, generealErrorMessage);
                }

            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Erro na requisição HTTP: {ex.Message}";
                _logger.LogError(ex.Message);
                return (default, errorMessage);
            }
            catch (JsonException ex)
            {
                string errorMessage = $"Erro na requisição JSON: {ex.Message}";
                _logger.LogError(ex.Message);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erro inesperado: {ex.Message}";
                _logger.LogError(ex.Message);
                return (default, errorMessage);
            }
        }

        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("acesstoken", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<HttpResponseMessage> PostResquest(string uri, HttpContent content)
        {
            var enderecoUrl = AppConfig.BaseUrl + uri;
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
