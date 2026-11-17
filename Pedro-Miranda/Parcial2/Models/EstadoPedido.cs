namespace Parcial2.Models
{
    // Recorrido normal: EnEspera -> Lavando -> ParaRetirar -> Entregado.
    // Cancelado es un estado final al que se llega desde EnEspera o desde Lavando.
    public enum EstadoPedido
    {
        EnEspera,
        Lavando,
        ParaRetirar,
        Entregado,
        Cancelado
    }
}
