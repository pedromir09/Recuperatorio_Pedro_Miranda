using System;
using System.Collections.Generic;
using System.Linq;

namespace Parcial2.Models
{
    public class Pedido : IEntidad
    {
        public const int MaximoPorItem = 20;

        public int Id { get; set; }
        public string Cliente { get; set; } = "";
        public string Telefono { get; set; } = "";       // se guarda solo con dígitos: 2235550101
        public string Observaciones { get; set; } = "";  // opcional: "sin suavizante", "mancha en la manga"
        public string Cuit { get; set; } = "";

        // Nombre de la forma de pago: "Efectivo", "Tarjeta", "Cuenta empresa".
        // En el JSON se guarda como TEXTO.
        public string FormaPago { get; set; } = "";
        public int Cuotas { get; set; } = 1;

        public List<ItemPedido> Items { get; set; } = new List<ItemPedido>();

        // Se congelan al confirmar el pedido. Ajuste negativo = descuento; positivo = recargo.
        public decimal Ajuste { get; set; }
        public decimal Total { get; set; }

        public EstadoPedido Estado { get; set; } = EstadoPedido.EnEspera;
        public DateTime FechaIngreso { get; set; } = DateTime.Now;

        // TODO 2 · Modelo y validaciones. Reglas completas en el README. Resumen:
        //  - AgregarItem: solo si el pedido está EnEspera (si no, InvalidOperationException);
        //    prenda obligatoria; cantidad entre 1 y MaximoPorItem; si la prenda ya está, se SUMA la cantidad.
        //  - CalcularSubtotal: suma de los subtotales de los ítems.
        //  - AvanzarEstado: EnEspera -> Lavando -> ParaRetirar -> Entregado. Un pedido finalizado no avanza.
        //  - Cancelar: solo si está EnEspera o Lavando.
        // Dato inválido: ArgumentException. Operación que no corresponde: InvalidOperationException.

        public void AgregarItem(Prenda prenda, int cantidad)
        {
            if (Estado != EstadoPedido.EnEspera)        
                throw new InvalidOperationException($"El pedido #{Id} está {Estado}.");

            if (prenda == null)
                throw new ArgumentException("Elegi una prenda.");

            if (cantidad < 1 || cantidad > MaximoPorItem)   
                throw new ArgumentException($"La cantidad tiene que estar entre 1 y {MaximoPorItem}.");

            ItemPedido existente = Items.Find(i => i.Descripcion == prenda.Nombre); 

            if (existente != null)
            {
                if (existente.Cantidad + cantidad > MaximoPorItem)
                    throw new ArgumentException($"No se pueden cargar mas de {MaximoPorItem} de {prenda.Nombre}.");
                existente.Cantidad += cantidad;
            }
            else
            {
                Items.Add(new ItemPedido       
                {
                    Descripcion = prenda.Nombre,
                    PrecioUnitario = prenda.Precio,
                    Cantidad = cantidad
                });
            }
        }

        public decimal CalcularSubtotal()
        {
            return Items.Sum(i => i.Subtotal);
        }

        public bool EstaFinalizado()
        {
            return Estado == EstadoPedido.Entregado || Estado == EstadoPedido.Cancelado;
        }

        public void AvanzarEstado()
        {
            if (EstaFinalizado())    
                throw new InvalidOperationException($"El pedido #{Id} esta {Estado}: no puede avanzar.");

            switch (Estado)
            {
                case EstadoPedido.EnEspera: Estado = EstadoPedido.Lavando; break;
                case EstadoPedido.Lavando: Estado = EstadoPedido.ParaRetirar; break;
                case EstadoPedido.ParaRetirar: Estado = EstadoPedido.Entregado; break;
            }
        }

        public void Cancelar()
        {
            if (Estado != EstadoPedido.EnEspera && Estado != EstadoPedido.Lavando)
                throw new InvalidOperationException($"El pedido #{Id} esta {Estado}: ya no se puede cancelar.");
        }
    }
}
