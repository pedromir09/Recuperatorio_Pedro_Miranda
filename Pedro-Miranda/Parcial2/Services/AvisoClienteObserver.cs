using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parcial2.Observer;

namespace Parcial2.Services
{
    public class AvisoClienteObserver : IPedidoObserver
    {
        private readonly string _ruta;

        public AvisoClienteObserver(string ruta)
        {
            _ruta = ruta;
        }

        public void Actualizar(EventoPedido evento)
        {
            PedidoTrabajo p = evento.Pedido;
            string texto = null;

            if (evento.Tipo == TipoEvento.Alta)
                texto = $"Hola {p.Cliente}, recibimos tu {p.Pedido} (orden #{p.Id}). Total: {p.Total:C0}.";
            else if (evento.Tipo == TipoEvento.CambioEstado && p.Estado == EventoPedido.ParaRetirar)
                texto = $"Tu pedido #{p.Id} ya esta listo para retirar. Total: {p.Total:C0}.";
            else if (evento.Tipo == TipoEvento.Cancelacion)
                texto = $"Tu pedido #{p.Id} fue cancelado.Podes pasar a buscar tus prendas";

            if (texto == null)          
                return;

            string linea = $"[{evento.Fecha:dd/MM HH:mm}] WhatsApp a {p.Cliente} ({p.Telefono}): {texto}";
            File.AppendAllText(_ruta, linea + Environment.NewLine);
        }
    }
}

