using WSM.AppLanches.UI.Services;
using WSM.AppLanches.UI.Validations;

namespace WSM.AppLanches.UI.Pages;

public partial class DetalhesPedidoPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validador;
    private bool _loginPageDisplay = false;
    public DetalhesPedidoPage(ApiService apiService, IValidator validador, int pedidoId, decimal precoTotal)
    {
        InitializeComponent();
        _apiService = apiService;
        _validador = validador;
        LblPrecoTotal.Text = " R$" + precoTotal;
        GetPedidosDetalhe(pedidoId);
    }

    private async Task GetPedidosDetalhe(int pedidoId)
    {
        try
        {
            var (pedidosDetalhes, errorMessage) = await _apiService.GetPedidosDetalhes(pedidoId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplay)
            {
                await DisplayLoginpage();
                return;
            }

            if (pedidosDetalhes.Count() == 0)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possivel obter detalhes do pedido.", "OK");
                return;
            }
            else
            {
                CvPedidosDetalhes.ItemsSource = pedidosDetalhes;
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro ao obter os detalhes. Tente mais tarde", "OK");
            return;
        }
    }

    private async Task DisplayLoginpage()
    {
        _loginPageDisplay = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validador));
    }
}