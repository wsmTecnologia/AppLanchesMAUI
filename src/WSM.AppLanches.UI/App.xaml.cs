using WSM.AppLanches.UI.Pages;
using WSM.AppLanches.UI.Services;

namespace WSM.AppLanches.UI
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;
        public App(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            MainPage = new NavigationPage(new InscricaoPage(_apiService));
            
        }
    }
}
