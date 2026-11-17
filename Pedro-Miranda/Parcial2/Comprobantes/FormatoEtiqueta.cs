using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parcial2.Models;

namespace Parcial2.Comprobantes
{
    public class FormatoEtiqueta : IFormatoComprobante
    {
        private readonly StringBuilder _sb = new StringBuilder();
        public void Encabezado(Pedido pedido)
        {
            _sb.AppendLine("==============================================");
            _sb.AppendLine("        LAVADERO LA PERLA");
           _sb.AppendLine("==============================================");
            _sb.AppendLine($"Pedido #{pedido.Id}   Cliente: {pedido.Cliente}");
        }

        public void Item(ItemPedido item)
        {
            _sb.AppendLine($"{item.Cantidad,3} x {item.Descripcion,-28}");
        }

        public void Totales(decimal subtotal, decimal ajuste, decimal total)
        {
            _sb.AppendLine("----------------------------------------------");
            _sb.AppendLine($"{"Subtotal",-34}{subtotal,12:C0}");
            _sb.AppendLine($"{(ajuste < 0 ? "Descuento" : "Recargo"),-34}{ajuste,12:C0}");
            _sb.AppendLine($"{"TOTAL",-34}{total,12:C0}");
            _sb.AppendLine("==============================================");
        }

        public string Resultado()
        {
            return _sb.ToString();
        }
    }
}
