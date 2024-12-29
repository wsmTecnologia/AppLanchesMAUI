using System.Collections.ObjectModel;
using WSM.AppLanches.UI.Models;
using WSM.AppLanches.UI.Services;
using WSM.AppLanches.UI.Validations;

namespace WSM.AppLanches.UI.Pages;

public partial class CarrinhoPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private ObservableCollection<CarrinhoCompraItem> ItensCarrinhoCompra = new ObservableCollection<CarrinhoCompraItem>();
    private bool isNavigationToEmptyCartPage = false;
    public CarrinhoPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        this._validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (IsNavigationToEmptyCartPage()) return;
        bool hasItems = await GetItensCarrinhoCompra();

        if (hasItems)
        {
            ExibirEndereco();
        }
        else
        {
            await NavegarParaCarrinhoVazio();
        }

        
    }

    private async Task NavegarParaCarrinhoVazio()
    {
        LblEndereco.Text = string.Empty;
        isNavigationToEmptyCartPage = true;
        await Navigation.PushAsync(new CarrinhoVazioPage(_apiService, _validator));
    }
    private bool IsNavigationToEmptyCartPage()
    {
        if (isNavigationToEmptyCartPage)
        {
            isNavigationToEmptyCartPage = false;
            return true;
        }
        return false;
    }
    private void ExibirEndereco()
    {
        bool enderecoSalvo = Preferences.ContainsKey("endereco");
        if (enderecoSalvo)
        {
            string nome = Preferences.Get("nome", string.Empty);
            string endereco = Preferences.Get("endereco", string.Empty);
            string telefone = Preferences.Get("telefone", string.Empty);
            LblEndereco.Text = $"{nome}\n{endereco}\n{telefone}";
        }
        else
        {
            LblEndereco.Text = $"Informe seu endereço";
        }
    }
    private async Task<bool> GetItensCarrinhoCompra()
    {
        try
        {
            var usuarioId = Preferences.Get("usuarioid", 0);
            var (itensCarrinhoCompra, errorMessage) = await _apiService.GetItensCarrinhoCompra(usuarioId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return false;
            }

            if (itensCarrinhoCompra == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possivel obter os itens.", "OK");
                return false;
            }
            ItensCarrinhoCompra.Clear();
            foreach (var item in itensCarrinhoCompra)
            {
                ItensCarrinhoCompra.Add(item);
            }
            CvCarrinho.ItemsSource = ItensCarrinhoCompra;
            AtualizarPrecoTotal();
            return itensCarrinhoCompra.Any();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return false;
        }
    }

    private void AtualizarPrecoTotal()
    {
        try
        {
            var precoTotal = ItensCarrinhoCompra.Sum(item => item.Preco * item.Quantidade);
            LblPrecoTotal.Text = precoTotal.ToString();
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Ocorreu um erro ao atualizar o preco: {ex.Message}", "OK");
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void BtnIncrementa_Clicked(object sender, EventArgs e)
    {
        if(sender is Button button && button.BindingContext is CarrinhoCompraItem carrinhoCompraItem)
        {
            carrinhoCompraItem.Quantidade++;
            AtualizarPrecoTotal();
            await _apiService.AtualizaQuantidadeItemCarrinho(carrinhoCompraItem.ProdutoId, "aumentar");
        }
    }

    private async void BtnDecrementa_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is CarrinhoCompraItem carrinhoCompraItem)
        {
            if(carrinhoCompraItem.Quantidade == 1){
                carrinhoCompraItem.Quantidade--;
                AtualizarPrecoTotal();
                await _apiService.AtualizaQuantidadeItemCarrinho(carrinhoCompraItem.ProdutoId, "diminuir");
            };
            
        }
    }

    private async void BtnDeletar_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is CarrinhoCompraItem carrinhoCompraItem)
        {
            bool resposta = await DisplayAlert("Confirmação", "Tem certeza qye deseja excluir este item do carrinho","Sim", "Não");
            if (resposta)
            {
                ItensCarrinhoCompra.Remove(carrinhoCompraItem);
                AtualizarPrecoTotal();
                await _apiService.AtualizaQuantidadeItemCarrinho(carrinhoCompraItem.ProdutoId, "deletar");
            };

        }
    }

    private void BtnEditarEndereco_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Endereco());
    }

    private async void TapConfirmarPedido_Tapped(object sender, TappedEventArgs e)
    {
        if(ItensCarrinhoCompra == null || !ItensCarrinhoCompra.Any())
        {
            await DisplayAlert("Informação", "Seu carrinho está vazio ou o pedido ja foi confirmado.", "OK");
            return;
        }

        var pedido = new Pedido()
        {
            Endereco = LblEndereco.Text,
            UsuarioId = Preferences.Get("usuarioid", 0),
            ValorTotal = Convert.ToDecimal(LblPrecoTotal.Text)
        };
        var response = await _apiService.ConfirmarPedido(pedido);

        if (response.HasError)
        {
            if(response.ErrorMessage == "Unauthorized")
            {
                await DisplayLoginPage();
                return;
            }
            await DisplayAlert("Opa !!!", $"Algo deu errado:{response.ErrorMessage}", "Cancelar");
            return;
        }

        ItensCarrinhoCompra.Clear();
        LblEndereco.Text = "Informe o seu endereço";
        LblPrecoTotal.Text = "0.00";

        await Navigation.PushAsync(new PedidoConfirmadoPage());

    }
}