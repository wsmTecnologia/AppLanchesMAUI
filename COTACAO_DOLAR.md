# Funcionalidade de Cotação do Dólar

Este documento descreve como usar a nova funcionalidade para buscar informações sobre a cotação do dólar americano (USD) em relação ao real brasileiro (BRL) na internet.

## Classe CotacaoMoeda

A classe `CotacaoMoeda` representa as informações da cotação:

```csharp
public class CotacaoMoeda
{
    public string? Base { get; set; }          // Moeda base (USD)
    public decimal Rate { get; set; }          // Taxa de conversão
    public string? Target { get; set; }        // Moeda alvo (BRL)
    public DateTime LastUpdate { get; set; }   // Data/hora da última atualização
}
```

## Método GetCotacaoDolar

O método `GetCotacaoDolar()` foi adicionado à classe `ApiService` para buscar a cotação atual do dólar:

```csharp
public async Task<(CotacaoMoeda? cotacao, string? ErrorMessage)> GetCotacaoDolar()
```

### Características:
- Utiliza a API gratuita exchangerate-api.com
- Retorna uma tupla com a cotação e mensagem de erro
- Segue o mesmo padrão de tratamento de erro dos outros métodos da classe
- Inclui logs informativos e de erro

### Exemplo de Uso:

```csharp
var (cotacao, erro) = await apiService.GetCotacaoDolar();

if (cotacao != null)
{
    Console.WriteLine($"1 USD = {cotacao.Rate:F4} BRL");
    Console.WriteLine($"Última atualização: {cotacao.LastUpdate}");
}
else
{
    Console.WriteLine($"Erro ao buscar cotação: {erro}");
}
```

## Exemplo de Implementação

A classe `CotacaoDolarExample` na pasta `Examples` demonstra diferentes formas de usar a funcionalidade:

1. **Exibir cotação**: Mostra como buscar e exibir as informações da cotação
2. **Conversão Real → Dólar**: Converte valores de reais para dólares
3. **Conversão Dólar → Real**: Converte valores de dólares para reais

### Exemplo de conversão:

```csharp
var exemplo = new CotacaoDolarExample(apiService);

// Converter R$ 100,00 para dólares
var valorEmDolares = await exemplo.ConverterReaisParaDolares(100);

// Converter $50,00 para reais
var valorEmReais = await exemplo.ConverterDolaresParaReais(50);
```

## Tratamento de Erros

O método segue o padrão estabelecido na classe `ApiService` para tratamento de erros:

- **HttpRequestException**: Problemas de conectividade ou rede
- **JsonException**: Problemas na desserialização da resposta
- **Exception**: Outros erros inesperados

Todos os erros são logados apropriadamente usando o `ILogger` configurado.

## API Externa Utilizada

- **Serviço**: exchangerate-api.com
- **Endpoint**: https://api.exchangerate-api.com/v4/latest/USD
- **Gratuito**: Sim, com limites de uso
- **Documentação**: https://exchangerate-api.com/docs

## Integração com a Aplicação

Para usar em uma página MAUI, injete o `ApiService` via construtor e chame o método conforme necessário:

```csharp
public partial class MinhaPage : ContentPage
{
    private readonly ApiService _apiService;

    public MinhaPage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    private async void BuscarCotacao_Clicked(object sender, EventArgs e)
    {
        var (cotacao, erro) = await _apiService.GetCotacaoDolar();
        
        if (cotacao != null)
        {
            LabelCotacao.Text = $"1 USD = {cotacao.Rate:F4} BRL";
        }
        else
        {
            await DisplayAlert("Erro", erro, "OK");
        }
    }
}
```