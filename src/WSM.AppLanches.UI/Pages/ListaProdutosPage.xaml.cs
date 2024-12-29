using WSM.AppLanches.UI.Models;
using WSM.AppLanches.UI.Services;
using WSM.AppLanches.UI.Validations;

namespace WSM.AppLanches.UI.Pages;

public partial class ListaProdutosPage : ContentPage
{
	private readonly ApiService _apiService;
	private readonly IValidator  _validator;
    private int _categoriaId;
	private bool _loginPageDisplayed = false;
    public ListaProdutosPage(int categoriaId, string categroiaNome, ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _categoriaId = categoriaId;
        Title = categroiaNome ?? "Produtos";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetListaProdutos(_categoriaId);
    }

    private async Task<IEnumerable<Produto>> GetListaProdutos(int categoriaId)
    {
        try
        {
            var (produtos, errorMessage) = await _apiService.GetProduto("categoria", categoriaId.ToString());

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Produto>();
            }

            if (produtos == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possivel obter os produtos.", "OK");
                return Enumerable.Empty<Produto>();
            }

            CvProdutos.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Produto>();
        }
    }

    private async Task DisplayLoginPage() 
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void CvProdutos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Produto;

        if (currentSelection is null) { return; }

        Navigation.PushAsync(new ProdutosDetalhePage(currentSelection.Id, currentSelection.Nome!, _apiService, _validator));

        ((CollectionView)sender).SelectedItem = null;
    }
}