using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Parcial2.Controllers;
using Parcial2.Models;
using Parcial2.Repository;
using Parcial2.Services;
using Parcial2.Views;

namespace Parcial2
{
    internal static class Program
    {
        // Composition Root: el ÚNICO lugar donde se arma el sistema.
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-AR");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-AR");

            string carpetaDatos = Path.Combine(AppContext.BaseDirectory, "Datos");
            Directory.CreateDirectory(carpetaDatos);

            // Los repositorios se crean UNA sola vez: inyección de dependencias directa, sin factory.
            IRepository<Pedido> pedidos = new JsonRepository<Pedido>(Path.Combine(carpetaDatos, "pedidos.json"));
            IRepository<Prenda> prendas = new JsonRepository<Prenda>(Path.Combine(carpetaDatos, "prendas.json"));

            var pedidoController = new PedidoController(pedidos, prendas);
            var comprobanteController = new ComprobanteController(pedidos);

            var formPrincipal = new FormPrincipal(pedidoController, comprobanteController);

            // TODO 7 · Observer: acá se decide QUIÉN escucha al pedidoController.
            //   - HistorialObserver
            //   - AvisoClienteObserver (archivo Datos/avisos_clientes.txt)
            //   - el formPrincipal (ya implementa IPedidoObserver)

            pedidoController.Suscribir(new HistorialObserver());
            pedidoController.Suscribir(new AvisoClienteObserver(Path.Combine(carpetaDatos, "avisos_clientes.txt")));
            pedidoController.Suscribir(formPrincipal);

            Application.Run(formPrincipal);
        }
    }
}
