using WSM.AppLanches.UI.Models;
using WSM.AppLanches.UI.Services;
using WSM.AppLanches.UI.Validations;
using Xamarin.Google.Crypto.Tink.Subtle;

namespace WSM.AppLanches.UI.Pages;

public partial class PerfilPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    public PerfilPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        this._validator = validator;
        LblNomeUsuario.Text = Preferences.Get("usuarioid", string.Empty);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ImgBtnPerfil.Source = await GetImagemPerfil();
    }

    private async Task<string> GetImagemPerfil()
    {
        string imagemPadrao = AppConfig.PerfilImagemPadrao;
        var (response, errorMessage) = await _apiService.GetImagemPerfilUsuario();

        if(errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    if (!_loginPageDisplayed)
                    {
                        await DisplayLoginPage();
                        return null;
                    };
                    break;
                default:
                    await DisplayAlert("Erro", errorMessage ?? "Não foi possivel obter a imagem.", "OK");
                    return imagemPadrao;
            }
        }
        if(response?.UrlImagem is not null)
        {
            return response.CaminhoImagem;
        }

        return imagemPadrao;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void TapPerguntas_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void ImgBtnPerfil_Clicked(object sender, EventArgs e)
    {

    }

    private void TapPedidos_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void MinhaConta_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void BtnLogout_Clicked(object sender, EventArgs e)
    {

    }
}