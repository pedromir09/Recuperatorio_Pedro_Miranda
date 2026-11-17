using System;
using System.IO;
using System.Text;
using Parcial2.Models;

namespace Parcial2.Comprobantes
{
    // PATRÓN FACTORY METHOD — Creador
    // Su responsabilidad PRINCIPAL es armar el comprobante (la lógica de negocio), UNA sola vez.
    // Lo único que delega a las subclases es QUÉ formato usar: el método fábrica CrearFormato().
    public abstract class GeneradorComprobante
    {
        public abstract string Descripcion { get; }   // "Comprobante de texto"
        public abstract string Extension { get; }     // "txt"

        protected abstract IFormatoComprobante CrearFormato();

        public void Generar(Pedido pedido, string ruta)
        {
            if (pedido.Estado == EstadoPedido.Cancelado)
                throw new InvalidOperationException($"El pedido #{pedido.Id} está cancelado: no se emite comprobante.");

            IFormatoComprobante formato = CrearFormato();   // <- acá se usa el Factory Method

            formato.Encabezado(pedido);
            foreach (ItemPedido item in pedido.Items)
                formato.Item(item);
            formato.Totales(pedido.CalcularSubtotal(), pedido.Ajuste, pedido.Total);

            File.WriteAllText(ruta, formato.Resultado(), Encoding.UTF8);
        }
    }
}
