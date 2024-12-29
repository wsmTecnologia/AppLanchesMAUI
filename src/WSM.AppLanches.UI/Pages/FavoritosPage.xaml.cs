using WSM.AppLanches.UI.Models;
using WSM.AppLanches.UI.Services;
using WSM.AppLanches.UI.Validations;

namespace WSM.AppLanches.UI.Pages;

public partial class FavoritosPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validador;
    private readonly FavoritosService _favoritosService = new FavoritosService();

    public FavoritosPage(ApiService apiService, IValidator validador)
    {
        InitializeComponent();
        _apiService = apiService;
        _validador = validador;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProdutosFavoritos();
    }

    private async Task GetProdutosFavoritos()
    {
        try
        {
            var produtosFavoritos = await _favoritosService.ReadAllAsync();
            if(produtosFavoritos is null || produtosFavoritos.Count == 0)
            {
                CvProdutos.ItemsSource = null;
                LblAviso.IsVisible = true;
            }
            else
            {
                CvProdutos.ItemsSource = produtosFavoritos;
                LblAviso.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
        }
    }
    private void CvProdutos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as ProdutoFavorito;

        if (currentSelection == null) { return; }

        Navigation.PushAsync(new ListaProdutosPage(currentSelection.ProdutoId, currentSelection.Nome!, _apiService, _validador));

        ((CollectionView)sender).SelectedItem = null;
    }
}