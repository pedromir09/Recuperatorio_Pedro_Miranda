using System;
using System.Linq;
using System.Windows.Forms;
using Parcial2.Controllers;
using Parcial2.Models;

namespace Parcial2.Views
{
    public partial class FormNuevoPedido : Form
    {
        private readonly PedidoController _controller;
        private readonly Pedido _borrador;      // el pedido "en edición" mientras se cargan las prendas

        public Pedido PedidoCreado { get; private set; }

        public FormNuevoPedido(PedidoController controller)
        {
            InitializeComponent();
            _controller = controller;
            _borrador = _controller.NuevoBorrador();

            // Factory: la lista de formas de pago sale del único lugar que conoce las estrategias.
            cmbPago.DataSource = _controller.FormasDePago();
            cmbPrenda.DataSource = _controller.ObtenerCatalogo();
            ActualizarControlesPago();
            ActualizarTotales();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cmbPrenda.SelectedItem is not Prenda prenda)
                return;

            try
            {
                _controller.AgregarItem(_borrador, prenda.Id, (int)nudCantidad.Value);
                RefrescarItems();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Revisá la prenda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo agregar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbPago_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarControlesPago();
            ActualizarTotales();
        }

        private void nudCuotas_ValueChanged(object sender, EventArgs e)
        {
            ActualizarTotales();
        }

        private void RefrescarItems()
        {
            lstItems.DataSource = null;
            lstItems.DataSource = _borrador.Items.ToList();
            ActualizarTotales();
        }

        private void ActualizarTotales()
        {
            decimal subtotal;
            try
            {
                subtotal = _borrador.CalcularSubtotal();
            }
            catch (Exception ex)
            {
                lblSubtotal.Text = "Subtotal: " + ex.Message;
                return;
            }
            lblSubtotal.Text = $"Subtotal: {subtotal:C0}";

            if (cmbPago.SelectedItem == null)
                return;

            try
            {
                decimal ajuste = _controller.CotizarPago(cmbPago.SelectedItem.ToString(), subtotal, (int)nudCuotas.Value);
                lblAjuste.Text = (ajuste < 0 ? "Descuento: " : "Recargo: ") + ajuste.ToString("C0");
                lblTotal.Text = $"TOTAL: {subtotal + ajuste:C0}";
            }
            catch (Exception ex)
            {
                // Por ejemplo: tarjeta en 4 cuotas, o cuenta corriente por más de $ 600.000
                lblAjuste.Text = "Pago: " + ex.Message;
                lblTotal.Text = "TOTAL: —";
            }
        }

        // TODO 6 (vista, 1 de 2): cuando cambia la forma de pago, habilitar los controles que correspondan.
        //   - nudCuotas habilitado solo si la forma de pago PermiteCuotas (si no, vuelve a 1).
        //   - txtCuit habilitado solo si RequiereCuit (si no, se vacía).
        // Preguntale a la estrategia con _controller.InfoPago(...): no compares el texto del ComboBox.
        private void ActualizarControlesPago()
        {

            if (cmbPago.SelectedItem == null)
                return;

            var info = _controller.InfoPago(cmbPago.SelectedItem.ToString());

            nudCuotas.Enabled = info.PermiteCuotas;
            if (!info.PermiteCuotas)
                nudCuotas.Value = 1;

            txtCuit.Enabled = info.RequiereCuit;
            if (!info.RequiereCuit)
                txtCuit.Text = "";
        }

        // TODO 6 (vista, 2 de 2): pedirle al controller que confirme el pedido con los datos del formulario.
        //   - Si sale bien: guardar el pedido en PedidoCreado, DialogResult = OK y cerrar.
        //   - Si los datos son inválidos o la operación no se puede hacer: mostrar el mensaje
        //     en un MessageBox y dejar el formulario abierto para que el usuario corrija.
        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Falta el TODO 6: conectar el botón Confirmar.", "Lavadero La Perla");
            try
            {
     
                PedidoCreado = _controller.ConfirmarPedido(
                    _borrador,
                    txtCliente.Text,
                    txtTelefono.Text,
                    txtObservaciones.Text,
                    txtCuit.Text,
                    (int)nudCuotas.Value,
                    cmbPago.SelectedItem.ToString());

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Porfa revisa los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "No se puede confirmar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
