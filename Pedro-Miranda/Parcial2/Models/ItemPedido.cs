namespace Parcial2.Models
{
    // Una línea del pedido: "3 x Camisa (lavado y planchado)".
    // Guarda la DESCRIPCIÓN y el PRECIO del momento: si mañana cambia la lista de precios,
    // lo que ya se le cobró al cliente no cambia.
    public class ItemPedido
    {
        public string Descripcion { get; set; } = "";
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }

        public decimal Subtotal => PrecioUnitario * Cantidad;

        public override string ToString()
        {
            return $"{Cantidad} x {Descripcion}  —  {Subtotal:C0}";
        }
    }
}
