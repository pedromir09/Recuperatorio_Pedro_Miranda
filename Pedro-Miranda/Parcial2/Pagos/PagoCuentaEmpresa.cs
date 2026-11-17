using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcial2.Pagos
{
    public class PagoCuentaEmpresa : IPagoStrategy
    {
        public string Nombre => "Cuenta empresa";
        public bool PermiteCuotas => false;
        public bool RequiereCuit => true;

        public decimal CalcularAjuste(decimal subtotal, int cuotas)
        {
            if (subtotal < 20000m)  
                throw new InvalidOperationException("La cuenta empresa admite pedidos desde $ 20.000.");
            return 100000m + subtotal * 0.08m;
        }
    }
}
