using System;
using System.Collections.Generic;
using System.Linq;
using Parcial2.Models;
using Parcial2.Observer;
using Parcial2.Pagos;
using Parcial2.Repository;

namespace Parcial2.Controllers
{
    // PATRÓN OBSERVER — este controller es el Notificador (Publisher).
    // Hace su trabajo (validar, calcular, guardar) y AVISA. No sabe quién escucha.
    public class PedidoController : IPedidoPublisher
    {
        public const int MaximoObservaciones = 100;

        private readonly IRepository<Pedido> _pedidos;
        private readonly IRepository<Prenda> _prendas;
        private readonly List<IPedidoObserver> _observadores = new List<IPedidoObserver>();

        // TODO 7 · Observer
        // Este controller tiene que ser el Notificador: guardar la lista de suscriptores,
        // permitir el alta y la baja, y avisarles a todos cuando algo pasa.

        public PedidoController(IRepository<Pedido> pedidos, IRepository<Prenda> prendas)
        {
            _pedidos = pedidos;
            _prendas = prendas;
        }

        public void Suscribir(IPedidoObserver observador)
        {
            //throw new NotImplementedException("Falta el TODO 7: PedidoController.Suscribir");
            if (!_observadores.Contains(observador))
                _observadores.Add(observador);
        }

        public void Desuscribir(IPedidoObserver observador)
        {
            //throw new NotImplementedException("Falta el TODO 7: PedidoController.Desuscribir");
            _observadores.Remove(observador);
        }

        private void Notificar(EventoPedido evento)
        {
            // TODO 7: avisarle a cada suscriptor. Hoy no hace nada: por eso la grilla no se refresca sola.
            foreach (IPedidoObserver observador in _observadores.ToList())
                observador.Actualizar(evento);
        }

        // ------------------------------------------------------------------ CONSULTAS

        public List<Prenda> ObtenerCatalogo()
        {
            return _prendas.LeerTodos().OrderBy(p => p.Categoria).ThenBy(p => p.Nombre).ToList();
        }

        public List<Pedido> ObtenerPedidos()
        {
            return _pedidos.LeerTodos().OrderByDescending(p => p.Id).ToList();
        }

        // Lo que tiene que mirar la persona de los lavarropas: lo que espera y lo que se está lavando.
        public List<Pedido> PedidosEnProceso()
        {
            return _pedidos.LeerTodos()
                .Where(p => p.Estado == EstadoPedido.EnEspera || p.Estado == EstadoPedido.Lavando)
                .OrderBy(p => p.Id)
                .ToList();
        }

        public int Contar(EstadoPedido estado)
        {
            return _pedidos.LeerTodos().Count(p => p.Estado == estado);
        }

        public decimal Facturacion()
        {
            return _pedidos.LeerTodos().Where(p => p.Estado != EstadoPedido.Cancelado).Sum(p => p.Total);
        }

        public List<string> FormasDePago()
        {
            return PagoFactory.TiposDisponibles();
        }

        // La vista lo usa para habilitar "Cuotas" y "CUIT" según la forma de pago elegida.
        public IPagoStrategy InfoPago(string tipo)
        {
            return PagoFactory.Crear(tipo);
        }

        // ------------------------------------------------------------------ ARMADO DEL PEDIDO

        public Pedido NuevoBorrador()
        {
            return new Pedido();
        }

        public void AgregarItem(Pedido borrador, int idPrenda, int cantidad)
        {
            Prenda prenda = _prendas.BuscarPorId(idPrenda);
            borrador.AgregarItem(prenda, cantidad);
        }

        // TODO 6 · Controller
        // CotizarPago: usá la factory para obtener la estrategia y delegale el cálculo (redondeado a 0 decimales).
        // ConfirmarPedido: validar, completar el borrador, calcular ajuste y total, guardar
        //                  y notificar el alta (TODO 7). Reglas en el README.

        public decimal CotizarPago(string tipoPago, decimal subtotal, int cuotas)
        {
            IPagoStrategy estrategia = PagoFactory.Crear(tipoPago);
            return Math.Round(estrategia.CalcularAjuste(subtotal, cuotas), 0);
        }

        public Pedido ConfirmarPedido(Pedido borrador, string cliente, string telefono,
                                      string observaciones, string cuit, int cuotas, string tipoPago)
        {
            // obligatorios
            if (string.IsNullOrWhiteSpace(cliente))
                throw new ArgumentException("Ingresa el nombre del cliente.");
            if (string.IsNullOrWhiteSpace(telefono))
                throw new ArgumentException("Ingresa el telefono del cliente.");
            if (borrador.Items.Count == 0)
                throw new ArgumentException("El pedido tiene que tener al menos una prenda.");
  
            string telefonoLimpio = (telefono ?? "").Replace(" ", "").ToUpper();
            if (telefonoLimpio.Length < 10 || telefonoLimpio.Length > 7 || !telefonoLimpio.All(char.IsLetterOrDigit))
                throw new ArgumentException(" (ej: 2235550101).");

            IPagoStrategy estrategia = PagoFactory.Crear(tipoPago);
            if (estrategia.RequiereCuit && string.IsNullOrWhiteSpace(cuit))   
                throw new ArgumentException($"Para pagar con \"{estrategia.Nombre}\" hace falta el CUIT.");

            
            int cuotasFinal = estrategia.PermiteCuotas ? cuotas : 1;
            decimal subtotal = borrador.CalcularSubtotal();
            decimal ajuste = Math.Round(estrategia.CalcularAjuste(subtotal, cuotasFinal), 0);

            borrador.Cliente = cliente.Trim();
            borrador.Telefono = telefono.Trim();
            borrador.Telefono = telefonoLimpio;
            borrador.Cuit = estrategia.RequiereCuit ? cuit.Trim() : "";
            borrador.Cuotas = cuotasFinal;
            borrador.FormaPago = estrategia.Nombre;
            borrador.Ajuste = ajuste;
            borrador.Total = subtotal + ajuste;
            borrador.Estado = EstadoPedido.EnEspera;
            borrador.FechaIngreso = DateTime.Now;

            
            _pedidos.Agregar(borrador);
            Notificar(new EventoPedido(TipoEvento.Alta, borrador, EstadoPedido.EnEspera));
            return borrador;
        }

        // ------------------------------------------------------------------ OPERACIONES

        public void AvanzarEstado(int id)
        {
            Pedido pedido = BuscarOFallar(id);
            EstadoPedido anterior = pedido.Estado;
            pedido.AvanzarEstado();
            _pedidos.Actualizar(pedido);

            Notificar(new EventoPedido(TipoEvento.CambioEstado, pedido, anterior));
        }

        public void Cancelar(int id)
        {
            Pedido pedido = BuscarOFallar(id);
            EstadoPedido anterior = pedido.Estado;
            pedido.Cancelar();
            _pedidos.Actualizar(pedido);

            Notificar(new EventoPedido(TipoEvento.Cancelacion, pedido, anterior));
        }

        private Pedido BuscarOFallar(int id)
        {
            return _pedidos.BuscarPorId(id) ?? throw new InvalidOperationException($"No existe el pedido #{id}.");
        }
    }
}
