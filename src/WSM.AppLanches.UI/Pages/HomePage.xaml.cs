using WSM.AppLanches.UI.Models;
using WSM.AppLanches.UI.Services;
using WSM.AppLanches.UI.Validations;

namespace WSM.AppLanches.UI.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validador;
    private bool _loginPageDisplay = false;
    public HomePage(ApiService apiService, IValidator validador)
    {
        InitializeComponent();
        _apiService = apiService;
        _validador = validador;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LblNomeUsuario.Text = "Olá, " + Preferences.Get("usuarionome", string.Empty);
        await GetListaCategorias();
        await GetMaisVendidos();
        await GetPopulares();
    }

    private async Task<IEnumerable<Categoria>> GetListaCategorias()
    {
        try
        {
            var (categorias, errorMessage) = await _apiService.GetCategorias();

            if(errorMessage == "Unauthorized" && !_loginPageDisplay)
            {
                await DisplayLoginpage();
                return Enumerable.Empty<Categoria>();
            }

            if (categorias == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possivel obter as categorias.", "OK");
                return Enumerable.Empty<Categoria>();
            }

            CvCategorias.ItemsSource = categorias;
            return categorias;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Categoria>();
        }
    }

    private async Task<IEnumerable<Produto>> GetMaisVendidos()
    {
        try
        {
            var (produtos, errorMessage) = await _apiService.GetProduto("maisvendido", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplay)
            {
                await DisplayLoginpage();
                return Enumerable.Empty<Produto>();
            }

            if (produtos == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possivel obter os produtos.", "OK");
                return Enumerable.Empty<Produto>();
            }

            CvMaisVendidos.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Produto>();
        }
    }

    private async Task<IEnumerable<Produto>> GetPopulares()
    {
        try
        {
            var (produtos, errorMessage) = await _apiService.GetProduto("popular", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplay)
            {
                await DisplayLoginpage();
                return Enumerable.Empty<Produto>();
            }

            if (produtos == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possivel obter os produtos.", "OK");
                return Enumerable.Empty<Produto>();
            }

            CvPopulares.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Produto>();
        }
    }

    private async Task DisplayLoginpage()
    {
        _loginPageDisplay = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validador));
    }

    private void CvCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Categoria;

        if (currentSelection == null) { return; }

        Navigation.PushAsync(new ListaProdutosPage(currentSelection.Id, currentSelection.Nome!, _apiService, _validador));

        ((CollectionView)sender).SelectedItem = null;
    }

    private void CvMaisVendidos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(sender is CollectionView collectionView)
        {
            NavigateToProdutoDetalhe(collectionView, e);
        }
    }

    private void CvPopulares_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            NavigateToProdutoDetalhe(collectionView, e);
        }
    }

    private void NavigateToProdutoDetalhe(CollectionView collectionView, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Produto;

        if (currentSelection == null) { return; }

        Navigation.PushAsync(new ProdutosDetalhePage(currentSelection.Id, currentSelection.Nome!, _apiService, _validador));

        collectionView.SelectedItem = null;
    }
}