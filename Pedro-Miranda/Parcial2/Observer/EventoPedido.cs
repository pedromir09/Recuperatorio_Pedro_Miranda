using System;
using Parcial2.Models;

namespace Parcial2.Observer
{
    // El "contexto" de la notificación: QUÉ pasó, a QUÉ pedido, desde qué estado y CUÁNDO.
    public class EventoPedido
    {
        public TipoEvento Tipo { get; }
        public Pedido Pedido { get; }
        public EstadoPedido EstadoAnterior { get; }
        public DateTime Fecha { get; }

        public EventoPedido(TipoEvento tipo, Pedido pedido, EstadoPedido estadoAnterior)
        {
            Tipo = tipo;
            Pedido = pedido;
            EstadoAnterior = estadoAnterior;
            Fecha = DateTime.Now;
        }

        public string Descripcion()
        {
            switch (Tipo)
            {
                case TipoEvento.Alta:
                    return $"Nuevo pedido #{Pedido.Id} - {Pedido.Cliente} - {Pedido.FormaPago} - {Pedido.Total:C0}";
                case TipoEvento.CambioEstado:
                    return $"Pedido #{Pedido.Id} ({Pedido.Cliente}): {EstadoAnterior} -> {Pedido.Estado}";
                default:
                    return $"Pedido #{Pedido.Id} ({Pedido.Cliente}) CANCELADO";
            }
        }
    }
}
