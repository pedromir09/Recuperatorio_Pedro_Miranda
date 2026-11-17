using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Parcial2.Comprobantes;
using Parcial2.Controllers;
using Parcial2.Models;
using Parcial2.Observer;
using Parcial2.Services;

namespace Parcial2.Views
{
    // El formulario principal es un suscriptor más: cuando algo cambia, el controller le avisa.
    public partial class FormPrincipal : Form, IPedidoObserver
    {
        private readonly PedidoController _controller;
        private readonly ComprobanteController _comprobantes;

        public FormPrincipal(PedidoController controller, ComprobanteController comprobantes)
        {
            InitializeComponent();
            _controller = controller;
            _comprobantes = comprobantes;
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            Refrescar();
            Bitacora.Instancia.Registrar("Sistema iniciado");
        }

        // ------------------------------------------------------------------ OBSERVER

        public void Actualizar(EventoPedido evento)
        {
            Refrescar();
        }

        // ------------------------------------------------------------------ BOTONES

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                using var form = new FormNuevoPedido(_controller);
                form.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnAvanzar_Click(object sender, EventArgs e)
        {
            int? id = IdSeleccionado();
            if (id == null) return;

            try
            {
                _controller.AvanzarEstado(id.Value);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            int? id = IdSeleccionado();
            if (id == null) return;

            if (MessageBox.Show($"¿Cancelar el pedido #{id}?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                _controller.Cancelar(id.Value);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnComprobante_Click(object sender, EventArgs e)
        {
            int? id = IdSeleccionado();
            if (id == null) return;

            List<GeneradorComprobante> generadores = _comprobantes.GeneradoresDisponibles();

            using var dialogo = new SaveFileDialog
            {
                Filter = string.Join("|", generadores.Select(g => $"{g.Descripcion} (*.{g.Extension})|*.{g.Extension}")),
                FileName = $"pedido_{id:0000}"
            };
            if (dialogo.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                GeneradorComprobante elegido = generadores[dialogo.FilterIndex - 1];   // FilterIndex arranca en 1
                _comprobantes.Generar(elegido, id.Value, dialogo.FileName);
                Bitacora.Instancia.Registrar($"Comprobante del pedido #{id} ({elegido.Descripcion})");
                MessageBox.Show("Comprobante generado.", "Lavadero La Perla", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnLavadero_Click(object sender, EventArgs e)
        {
            // Probá abrir DOS pantallas de lavado: las dos se tienen que actualizar solas.
            try
            {
                var lavado = new FormLavadero(_controller);
                lavado.Show(this);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnBitacora_Click(object sender, EventArgs e)
        {
            using var form = new FormBitacora();
            form.ShowDialog(this);
        }

        // ------------------------------------------------------------------ AUXILIARES

        private void Refrescar()
        {
            try
            {
                dgvPedidos.DataSource = _controller.ObtenerPedidos()
                    .Select(o => new
                    {
                        o.Id,
                        o.Cliente,
                        Pago = o.Cuotas > 1 ? $"{o.FormaPago} ({o.Cuotas} cuotas)" : o.FormaPago,
                        Prendas = o.Items.Sum(i => i.Cantidad),
                        Total = o.Total.ToString("C0"),
                        Estado = o.Estado.ToString(),
                        Ingreso = o.FechaIngreso.ToString("dd/MM HH:mm"),
                        o.Observaciones
                    })
                    .ToList();

                lblResumen.Text =
                    $"En espera: {_controller.Contar(EstadoPedido.EnEspera)}   ·   " +
                    $"Lavando: {_controller.Contar(EstadoPedido.Lavando)}   ·   " +
                    $"Para retirar: {_controller.Contar(EstadoPedido.ParaRetirar)}   ·   " +
                    $"Entregados: {_controller.Contar(EstadoPedido.Entregado)}   ·   " +
                    $"Facturado: {_controller.Facturacion():C0}";
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private int? IdSeleccionado()
        {
            if (dgvPedidos.CurrentRow == null)
            {
                MessageBox.Show("Seleccioná un pedido de la lista.", "Lavadero La Perla",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
            return (int)dgvPedidos.CurrentRow.Cells["Id"].Value;
        }

        private void MostrarError(Exception ex)
        {
            Bitacora.Instancia.Registrar("ERROR: " + ex.Message);
            MessageBox.Show(ex.Message, "No se pudo completar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
