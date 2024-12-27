using WSM.AppLanches.UI.Models;
using WSM.AppLanches.UI.Services;
using WSM.AppLanches.UI.Validations;

namespace WSM.AppLanches.UI.Pages;

public partial class ProdutosDetalhePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _produtoId;
    private bool _loginPageDisplayed = false;
    public ProdutosDetalhePage(int produtoId, string produtoNome, ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _produtoId = produtoId;
        Title = produtoNome ?? "Detalhe do Produto";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProdutoDetalhes(_produtoId);
    }
    private async Task<Produto?> GetProdutoDetalhes(int idProduto)
    {
        try
        {
            var (produtoDetalhe, errorMessage) = await _apiService.GetProdutoDetalhe(idProduto);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return null;
            }

            if (produtoDetalhe == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possivel obter os produtos.", "OK");
                return null;
            }

            if (produtoDetalhe != null)
            {
                ImagemProduto.Source = produtoDetalhe.CaminhoImagem;
                LblProdutoNome.Text = produtoDetalhe.Nome;
                LblProdutoPreco.Text = produtoDetalhe.Preco.ToString();
                LblProdutoDescricao.Text = produtoDetalhe.Detalhe;
                LblPrecoTotal.Text = produtoDetalhe.Preco.ToString();
            }
            else
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter os detalhes do produto.", "OK");
                return null;
            }
            return produtoDetalhe;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return null;
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void ImagemBtnFavorito_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnRemove_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantidade.Text, out int qtde) && decimal.TryParse(LblProdutoPreco.Text, out decimal precoUnitario))
        {
            qtde = Math.Max(1, qtde - 1);
            LblQuantidade.Text = qtde.ToString();

            var precoTotal = qtde * precoUnitario;
            LblPrecoTotal.Text = precoTotal.ToString();
        }
        else
        {
            DisplayAlert("Erro", $"Valores inválidos", "OK");
        }
    }

    private void BtnAdiciona_Clicked(object sender, EventArgs e)
    {
        if(int.TryParse(LblQuantidade.Text, out int qtde) && decimal.TryParse(LblProdutoPreco.Text, out decimal precoUnitario))
        {
            qtde++;
            LblQuantidade.Text = qtde.ToString();

            var precoTotal = qtde * precoUnitario;
            LblPrecoTotal.Text = precoTotal.ToString();
        }
        else
        {
            DisplayAlert("Erro", $"Valores inválidos", "OK");
        }
    }

    private async void BtnIncluirNoCarrinho_Clicked(object sender, EventArgs e)
    {
        try
        {
            var carrinhoCompra = new CarrinhoCompra()
            {
                Quantidade = Convert.ToInt32(LblQuantidade.Text),
                PrecoUnitario = Convert.ToDecimal(LblProdutoPreco.Text),
                ValorTotal = Convert.ToDecimal(LblPrecoTotal.Text),
                ProdutoId = _produtoId,
                ClienteId = Preferences.Get("usuarioid", 0)
            };
            var response = await _apiService.AdicionaItemNoCarrinho(carrinhoCompra);
            if (response.Data)
            {
                await DisplayAlert("Sucesso", $"Item adicionado ao carrinho !", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Erro", $"Falha ao adicionar item: {response.ErrorMessage}", "OK");
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro:{ex.Message}", "OK");
        }
    }
}