using WSM.AppLanches.UI.Models;
using WSM.AppLanches.UI.Services;
using WSM.AppLanches.UI.Validations;

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
        LblNomeUsuario.Text = Preferences.Get("usuarionome", string.Empty);
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

    private async void ImgBtnPerfil_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imagemArray = await SelecionarImagemAsync();
            if(imagemArray is null)
            {
                await DisplayAlert("Erro", "Não foi possivel carregar a imagem", "OK");
            }
            ImgBtnPerfil.Source = ImageSource.FromStream(() => new MemoryStream(imagemArray));

            var response = await _apiService.UploadImagemUsuario(imagemArray);
            if (response.Data)
            {
                await DisplayAlert("", "Imagem enviada com sucesso", "OK");
            }
            else
            {
                await DisplayAlert("Erro", response.ErrorMessage ?? "Ocorreu um erro desconhecido", "Cancela");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
        }
    }

    private async Task<byte[]?> SelecionarImagemAsync()
    {
        try
        {
            var arquivo = await MediaPicker.PickPhotoAsync();
            if (arquivo is null) return null;

            using(var stream = await arquivo.OpenReadAsync())
            using(var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Erro", $"A funcionalidade não é suportada no dispositvo", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Erro", $"Permissão não concedida pas acessar a camera ou galeria", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao selecionar a imagem: {ex.Message}", "OK");
        }
        return null;
    }
    private void TapPedidos_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new PedidosPage(_apiService, _validator));
    }

    private void MinhaConta_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void BtnLogout_Clicked(object sender, EventArgs e)
    {

    }
}