using SQLite;

namespace WSM.AppLanches.UI.Models
{
    public class ProdutoFavorito
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string? Nome { get; set; }
        public string? Detalhe { get; set; }
        public string? ImagemUrl { get; set; }
        public decimal Preco { get; set; }
        public bool IsFavorito { get; set; }
    }
}
