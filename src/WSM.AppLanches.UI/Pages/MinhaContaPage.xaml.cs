using WSM.AppLanches.UI.Models;
using WSM.AppLanches.UI.Services;

namespace WSM.AppLanches.UI.Pages;

public partial class MinhaContaPage : ContentPage
{
	private const string NomeUsuarioKey = "usuarionome";
    private const string EmailUsaurioKey = "usuarioemail";
    private const string TelefoneUsaurioKey = "usuariotelefone";
    private readonly ApiService _apiService;
    public MinhaContaPage(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ImgBtnPerfil.Source = await GetImagemPerfilAsync();
    }

    private void CarregarInformcoesUsaurio()
    {
        LblNomeUsuario.Text = Preferences.Get(NomeUsuarioKey, string.Empty);
        EntNome.Text = LblNomeUsuario.Text;
        EntEmail.Text = Preferences.Get(EmailUsaurioKey, string.Empty);
        EntFone.Text = Preferences.Get(TelefoneUsaurioKey, string.Empty);
    }

    private async Task<string?> GetImagemPerfilAsync()
    {
        string imagemPadrao = AppConfig.PerfilImagemPadrao;
        var (response, errorMessage) = await _apiService.GetImagemPerfilUsuario();
        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    await DisplayAlert("Erro", "Não autorizado", "OK");
                    return imagemPadrao;
                default:
                    await DisplayAlert("Erro", errorMessage ?? "Não foi possivel obter a imagem.", "OK");
                    return imagemPadrao;
            }
        }
        if(response?.UrlImagem is not null)
        {
            return response?.CaminhoImagem;
        }

        return imagemPadrao;
    }
    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        Preferences.Set(NomeUsuarioKey, EntNome.Text);
        Preferences.Set(EmailUsaurioKey, EntEmail.Text);
        Preferences.Set(TelefoneUsaurioKey, EntFone.Text);
        await DisplayAlert("Informações Salva", "Suas informações foram salvas com sucesso !!", "OK");
    }
}