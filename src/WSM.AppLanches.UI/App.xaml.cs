using WSM.AppLanches.UI.Pages;
using WSM.AppLanches.UI.Services;
using WSM.AppLanches.UI.Validations;

namespace WSM.AppLanches.UI
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        private readonly FavoritosService _favoritosService;

        public App(ApiService apiService, IValidator validator, FavoritosService favoritosService)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
            _favoritosService = favoritosService;
            MainPage = new NavigationPage(new InscricaoPage(_apiService, _validator));
        }
            
    }
}
