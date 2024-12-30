using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSM.AppLanches.UI.Models
{
    public class PedidosPorUsuario
    {
        public int Id { get; set; }
        public decimal PedidoTotal { get; set; }
        public DateTime DataPedido { get; set; }
    }
}
