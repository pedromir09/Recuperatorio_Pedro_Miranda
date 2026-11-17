namespace Parcial2.Observer
{
    // PATRÓN OBSERVER — interfaz Suscriptor
    // TODO 7 · Observer: creá en esta carpeta HistorialObserver y AvisoClienteObserver (reglas en el README).
    public interface IPedidoObserver
    {
        void Actualizar(EventoPedido evento);
    }
}
