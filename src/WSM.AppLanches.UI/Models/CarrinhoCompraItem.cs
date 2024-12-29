using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WSM.AppLanches.UI.Models
{
    public class CarrinhoCompraItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public decimal Preco { get; set; }
        public decimal ValorTotal { get; set; }
        private int quantidade;
        public int Quantidade
        {
            get { return quantidade; }
            set
            {
                if(Quantidade != value)
                {
                    quantidade = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ProdutoId { get; set; }
        public string? ProdutoNome { get; set; }
        public string? UrlImagem { get; set; }
        public string CaminhoImagem => AppConfig.BaseUrl + UrlImagem;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName= null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
