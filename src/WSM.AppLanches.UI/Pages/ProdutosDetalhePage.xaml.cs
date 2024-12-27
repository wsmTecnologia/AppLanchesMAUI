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
        Title = produtoNome ?? "Produtos";
    }
}