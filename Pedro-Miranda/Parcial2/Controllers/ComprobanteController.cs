using System;
using System.Collections.Generic;
using Parcial2.Comprobantes;
using Parcial2.Models;
using Parcial2.Repository;

namespace Parcial2.Controllers
{
    public class ComprobanteController
    {
        private readonly IRepository<Pedido> _pedidos;

        public ComprobanteController(IRepository<Pedido> pedidos)
        {
            _pedidos = pedidos;
        }

        // El SaveFileDialog arma sus filtros con esta lista.
        // Formato nuevo = una clase creadora nueva + UNA línea acá.
        public List<GeneradorComprobante> GeneradoresDisponibles()
        {
            return new List<GeneradorComprobante>
            {
                new GeneradorComprobanteTxt()
                // TODO 8 · Factory Method: sumá acá el generador de la etiqueta (una línea).
            };
        }

        public void Generar(GeneradorComprobante generador, int idPedido, string ruta)
        {
            Pedido pedido = _pedidos.BuscarPorId(idPedido)
                ?? throw new InvalidOperationException($"No existe el pedido #{idPedido}.");

            generador.Generar(pedido, ruta);
        }
    }
}
