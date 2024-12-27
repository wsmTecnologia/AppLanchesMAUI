using WSM.AppLanches.UI.Pages;
using WSM.AppLanches.UI.Services;
using WSM.AppLanches.UI.Validations;

namespace WSM.AppLanches.UI
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validador;
        public AppShell(ApiService apiService, IValidator validador)
        {
            InitializeComponent();
            _apiService = apiService;
            _validador = validador;
            ConfigureShell();
        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_apiService, _validador);
            var carrinhoPage = new CarrinhoPage();
            var favoritoPage = new FavoritosPage();
            var perfilPage = new PerfilPage();

            Items.Add(new TabBar
            {
                Items =
                {
                    new ShellContent{Title="Home", Icon="home", Content=homePage},
                    new ShellContent{Title="Carrinho", Icon="cart", Content=carrinhoPage},
                    new ShellContent{Title="Favoritos", Icon="favoritos", Content=favoritoPage},
                    new ShellContent{Title="Perfil", Icon="perfil", Content=perfilPage}
                }
            });
        }
    }
}
