using System;
using System.Drawing;
using System.Windows.Forms;

namespace Parcial2.Views
{
    partial class FormPrincipal
    {
        /// <summary>
        ///  Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Limpiar los recursos que se estén usando.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            lblTitulo = new Label();
            dgvPedidos = new DataGridView();
            lblResumen = new Label();
            btnNuevo = new Button();
            btnAvanzar = new Button();
            btnCancelar = new Button();
            btnComprobante = new Button();
            btnLavadero = new Button();
            btnBitacora = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvPedidos).BeginInit();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitulo.Location = new Point(12, 9);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Lavadero La Perla · Pedidos";
            // 
            // dgvPedidos
            // 
            dgvPedidos.AllowUserToAddRows = false;
            dgvPedidos.AllowUserToDeleteRows = false;
            dgvPedidos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPedidos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPedidos.Location = new Point(12, 50);
            dgvPedidos.MultiSelect = false;
            dgvPedidos.Name = "dgvPedidos";
            dgvPedidos.ReadOnly = true;
            dgvPedidos.RowHeadersVisible = false;
            dgvPedidos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPedidos.Size = new Size(876, 380);
            dgvPedidos.TabIndex = 1;
            // 
            // lblResumen
            // 
            lblResumen.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblResumen.Location = new Point(12, 440);
            lblResumen.Name = "lblResumen";
            lblResumen.Size = new Size(876, 23);
            lblResumen.TabIndex = 2;
            lblResumen.Text = "Resumen";
            // 
            // btnNuevo
            // 
            btnNuevo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnNuevo.Location = new Point(12, 475);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(135, 36);
            btnNuevo.TabIndex = 3;
            btnNuevo.Text = "Nuevo pedido";
            btnNuevo.UseVisualStyleBackColor = true;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // btnAvanzar
            // 
            btnAvanzar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAvanzar.Location = new Point(157, 475);
            btnAvanzar.Name = "btnAvanzar";
            btnAvanzar.Size = new Size(135, 36);
            btnAvanzar.TabIndex = 4;
            btnAvanzar.Text = "Avanzar estado";
            btnAvanzar.UseVisualStyleBackColor = true;
            btnAvanzar.Click += btnAvanzar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCancelar.Location = new Point(302, 475);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(135, 36);
            btnCancelar.TabIndex = 5;
            btnCancelar.Text = "Cancelar pedido";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // btnComprobante
            // 
            btnComprobante.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnComprobante.Location = new Point(447, 475);
            btnComprobante.Name = "btnComprobante";
            btnComprobante.Size = new Size(135, 36);
            btnComprobante.TabIndex = 6;
            btnComprobante.Text = "Comprobante";
            btnComprobante.UseVisualStyleBackColor = true;
            btnComprobante.Click += btnComprobante_Click;
            // 
            // btnLavadero
            // 
            btnLavadero.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLavadero.Location = new Point(592, 475);
            btnLavadero.Name = "btnLavadero";
            btnLavadero.Size = new Size(135, 36);
            btnLavadero.TabIndex = 7;
            btnLavadero.Text = "Pantalla lavado";
            btnLavadero.UseVisualStyleBackColor = true;
            btnLavadero.Click += btnLavadero_Click;
            // 
            // btnBitacora
            // 
            btnBitacora.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnBitacora.Location = new Point(737, 475);
            btnBitacora.Name = "btnBitacora";
            btnBitacora.Size = new Size(135, 36);
            btnBitacora.TabIndex = 8;
            btnBitacora.Text = "Bitácora";
            btnBitacora.UseVisualStyleBackColor = true;
            btnBitacora.Click += btnBitacora_Click;
            // 
            // FormPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            ClientSize = new Size(900, 525);
            Controls.Add(lblTitulo);
            Controls.Add(dgvPedidos);
            Controls.Add(lblResumen);
            Controls.Add(btnNuevo);
            Controls.Add(btnAvanzar);
            Controls.Add(btnCancelar);
            Controls.Add(btnComprobante);
            Controls.Add(btnLavadero);
            Controls.Add(btnBitacora);
            MinimumSize = new Size(916, 400);
            Name = "FormPrincipal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Lavadero La Perla — Pedidos";
            Load += FormPrincipal_Load;
            ((System.ComponentModel.ISupportInitialize)dgvPedidos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitulo;
        private DataGridView dgvPedidos;
        private Label lblResumen;
        private Button btnNuevo;
        private Button btnAvanzar;
        private Button btnCancelar;
        private Button btnComprobante;
        private Button btnLavadero;
        private Button btnBitacora;
    }
}
