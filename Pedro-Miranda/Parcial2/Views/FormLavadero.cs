using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Parcial2.Controllers;
using Parcial2.Models;
using Parcial2.Observer;

namespace Parcial2.Views
{
    // Pantalla de la zona de lavado. Puede haber varias abiertas a la vez (una por cada lavarropas).
    // PATRÓN OBSERVER — suscriptor que entra y sale en tiempo de ejecución.
    public partial class FormLavadero : Form, IPedidoObserver
    {
        private readonly PedidoController _controller;

        public FormLavadero(PedidoController controller)
        {
            InitializeComponent();
            _controller = controller;
            Refrescar();

            _controller.Suscribir(this);

            // TODO 7: esta pantalla tiene que enterarse sola de cada cambio. ¿Qué falta acá?
        }

        public void Actualizar(EventoPedido evento)
        {
            Refrescar();
            lstEventos.Items.Insert(0, $"{evento.Fecha:HH:mm:ss}  {evento.Descripcion()}");
        }

        private void FormLavadero_FormClosed(object sender, FormClosedEventArgs e)
        {
            _controller.Desuscribir(this);
            // TODO 7: ¿qué tiene que pasar cuando se cierra una pantalla de lavado?
        }

        private void Refrescar()
        {
            List<Pedido> enProceso = _controller.PedidosEnProceso();

            lstPendientes.Items.Clear();
            foreach (Pedido pedido in enProceso)
            {
                lstPendientes.Items.Add($"#{pedido.Id}  {pedido.Estado.ToString().ToUpper()}  ·  {pedido.Cliente}");
                foreach (ItemPedido item in pedido.Items)
                    lstPendientes.Items.Add($"        {item.Cantidad} x {item.Descripcion}");
                if (!string.IsNullOrWhiteSpace(pedido.Observaciones))
                    lstPendientes.Items.Add($"        Obs.: {pedido.Observaciones}");
            }

            lblPendientes.Text = $"Pedidos en proceso: {enProceso.Count}";
        }
    }
}
