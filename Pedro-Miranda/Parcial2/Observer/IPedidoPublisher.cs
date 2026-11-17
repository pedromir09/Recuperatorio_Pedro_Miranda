namespace Parcial2.Observer
{
    // PATRÓN OBSERVER — interfaz Notificador (Publisher): alta y baja de suscriptores.
    public interface IPedidoPublisher
    {
        void Suscribir(IPedidoObserver observador);
        void Desuscribir(IPedidoObserver observador);
    }
}
