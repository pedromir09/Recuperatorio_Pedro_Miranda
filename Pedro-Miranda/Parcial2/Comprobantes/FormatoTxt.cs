using System.Text;
using Parcial2.Models;

namespace Parcial2.Comprobantes
{
    // Producto concreto: comprobante de texto plano.
    public class FormatoTxt : IFormatoComprobante
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public void Encabezado(Pedido pedido)
        {
            _sb.AppendLine("==============================================");
            _sb.AppendLine("        LAVADERO LA PERLA");
            _sb.AppendLine("        Mar del Plata");
            _sb.AppendLine("==============================================");
            _sb.AppendLine($"Pedido #{pedido.Id}   {pedido.FechaIngreso:dd/MM/yyyy HH:mm}");
            _sb.AppendLine($"Cliente: {pedido.Cliente}");
            _sb.AppendLine($"Teléfono: {pedido.Telefono}");
            _sb.AppendLine($"Pago: {pedido.FormaPago}" + (pedido.Cuotas > 1 ? $" en {pedido.Cuotas} cuotas" : ""));
            if (!string.IsNullOrWhiteSpace(pedido.Cuit))
                _sb.AppendLine($"CUIT: {pedido.Cuit}");
            if (!string.IsNullOrWhiteSpace(pedido.Observaciones))
                _sb.AppendLine($"Obs.: {pedido.Observaciones}");
            _sb.AppendLine("----------------------------------------------");
        }

        public void Item(ItemPedido item)
        {
            _sb.AppendLine($"{item.Cantidad,3} x {item.Descripcion,-28}{item.Subtotal,12:C0}");
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
