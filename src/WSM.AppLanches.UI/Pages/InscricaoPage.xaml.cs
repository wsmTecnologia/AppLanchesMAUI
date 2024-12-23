using WSM.AppLanches.UI.Services;
using WSM.AppLanches.UI.Validations;

namespace WSM.AppLanches.UI.Pages;

public partial class InscricaoPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public InscricaoPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        this._validator = validator;
    }


    private async void BtnSignup_Clicked(object sender, EventArgs e)
    {
        if(await _validator.Validar(EntNome.Text, EntEmail.Text, EntPhone.Text, EntPassword.Text))
        {
            var response = await _apiService.RegistrarUSuario(EntEmail.Text, EntPassword.Text, EntPhone.Text, EntNome.Text);

            if (!response.HasError)
            {
                await DisplayAlert("Aviso", "Sua conta foi criado com sucesso!", "OK");
                await Navigation.PushAsync(new LoginPage(_apiService, _validator));
            }
            else
            {
                await DisplayAlert("Erro", "Algo deu errado", "Cancelar");
            }
        }
        else
        {
            string mensagemErro = "";
            mensagemErro += _validator.NomeErro != null ? $"\n- {_validator.NomeErro}" : "";
            mensagemErro += _validator.EmailErro != null ? $"\n- {_validator.EmailErro}" : "";
            mensagemErro += _validator.TelefoneErro != null ? $"\n- {_validator.TelefoneErro}" : "";
            mensagemErro += _validator.SenhaErro != null ? $"\n- {_validator.SenhaErro}" : "";

            await DisplayAlert("Erro", mensagemErro, "OK");
        }       

    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}