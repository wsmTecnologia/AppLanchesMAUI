using Microsoft.Maui.Controls;
using WSM.AppLanches.UI.Models;
using WSM.AppLanches.UI.Services;
using WSM.AppLanches.UI.Validations;

namespace WSM.AppLanches.UI.Pages;

public partial class PedidosPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validador;
    private bool _loginPageDisplay = false;
    public PedidosPage(ApiService apiService, IValidator validador)
    {
        InitializeComponent();
        _apiService = apiService;
        _validador = validador;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetListaPedidos();
    }

    private async Task GetListaPedidos()
    {
        try
        {
            var (pedidos, errorMessage) = await _apiService.GetPedidosPorUsuario(Preferences.Get("usuarioid",0));

            if (errorMessage == "Unauthorized" && !_loginPageDisplay)
            {
                await DisplayLoginpage();
                return;
            }

            if (pedidos == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não existe pedidos para o cliente.", "OK");
                return;
            }
            if(pedidos.Count() == 0)
            {
                await DisplayAlert("Erro", $"Não foi possivel obter os pedidos", "OK");
                return;
            }
            else
            {
                CvPedidos.ItemsSource = pedidos;
            }
           
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro ao obter os pedidos. Tente mais tarde", "OK");
            return;
        }
    }
    private async Task DisplayLoginpage()
    {
        _loginPageDisplay = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validador));
    }

    private void CvPedidos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as PedidosPorUsuario;

        if (currentSelection == null) { return; }

        Navigation.PushAsync(new DetalhesPedidoPage(_apiService, _validador, currentSelection.Id, currentSelection.PedidoTotal));

        ((CollectionView) sender).SelectedItem = null;
    }
}