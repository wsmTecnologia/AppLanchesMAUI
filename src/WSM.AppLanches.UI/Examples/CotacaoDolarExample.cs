using WSM.AppLanches.UI.Models;
using WSM.AppLanches.UI.Services;

namespace WSM.AppLanches.UI.Examples
{
    /// <summary>
    /// Classe de exemplo mostrando como usar o método GetCotacaoDolar do ApiService
    /// para buscar informações sobre a cotação do dólar em reais.
    /// </summary>
    public class CotacaoDolarExample
    {
        private readonly ApiService _apiService;

        public CotacaoDolarExample(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Exemplo de como buscar e exibir a cotação do dólar
        /// </summary>
        public async Task ExibirCotacaoDolar()
        {
            try
            {
                // Chama o método para buscar a cotação do dólar
                var (cotacao, errorMessage) = await _apiService.GetCotacaoDolar();

                if (cotacao != null)
                {
                    // Sucesso - exibe as informações da cotação
                    Console.WriteLine("=== COTAÇÃO DO DÓLAR ===");
                    Console.WriteLine($"Moeda Base: {cotacao.Base}");
                    Console.WriteLine($"Moeda Alvo: {cotacao.Target}");
                    Console.WriteLine($"Taxa de Conversão: 1 USD = {cotacao.Rate:F4} BRL");
                    Console.WriteLine($"Última Atualização: {cotacao.LastUpdate:dd/MM/yyyy HH:mm:ss}");
                    Console.WriteLine($"Valor para R$ 100,00: ${(100 / cotacao.Rate):F2} USD");
                    Console.WriteLine($"Valor para $ 100,00: R$ {(100 * cotacao.Rate):F2} BRL");
                }
                else
                {
                    // Erro - exibe a mensagem de erro
                    Console.WriteLine($"Erro ao buscar cotação: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Exemplo de como usar a cotação para converter valores
        /// </summary>
        /// <param name="valorReais">Valor em reais a ser convertido para dólares</param>
        /// <returns>Valor convertido em dólares ou null se houver erro</returns>
        public async Task<decimal?> ConverterReaisParaDolares(decimal valorReais)
        {
            var (cotacao, errorMessage) = await _apiService.GetCotacaoDolar();

            if (cotacao != null)
            {
                return valorReais / cotacao.Rate;
            }
            else
            {
                Console.WriteLine($"Erro ao buscar cotação para conversão: {errorMessage}");
                return null;
            }
        }

        /// <summary>
        /// Exemplo de como usar a cotação para converter valores
        /// </summary>
        /// <param name="valorDolares">Valor em dólares a ser convertido para reais</param>
        /// <returns>Valor convertido em reais ou null se houver erro</returns>
        public async Task<decimal?> ConverterDolaresParaReais(decimal valorDolares)
        {
            var (cotacao, errorMessage) = await _apiService.GetCotacaoDolar();

            if (cotacao != null)
            {
                return valorDolares * cotacao.Rate;
            }
            else
            {
                Console.WriteLine($"Erro ao buscar cotação para conversão: {errorMessage}");
                return null;
            }
        }
    }
}