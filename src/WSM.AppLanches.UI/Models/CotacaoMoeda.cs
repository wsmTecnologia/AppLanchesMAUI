namespace WSM.AppLanches.UI.Models
{
    public class CotacaoMoeda
    {
        public string? Base { get; set; }
        public decimal Rate { get; set; }
        public string? Target { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    // Modelo para resposta da API externa (exchangerate-api.com)
    public class ExchangeRateApiResponse
    {
        public string? Result { get; set; }
        public string? Base_Code { get; set; }
        public string? Target_Code { get; set; }
        public decimal Conversion_Rate { get; set; }
        public long Time_Last_Update_Unix { get; set; }
    }
}