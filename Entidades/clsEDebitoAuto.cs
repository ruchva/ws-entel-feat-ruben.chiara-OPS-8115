using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class clsEDebitoAuto
    {
        public int pCodigo { get; set; }
        public int pCodConvenio { get; set; }
        public int pCodPlaza { get; set; }
        public DateTime pFecha { get; set; }
        public string pUsuario { get; set; }
        public List<clsEDebitoAutoDet> pListDetalle {get; set;}

        public clsEDebitoAuto()
        {
            this.pListDetalle = new List<clsEDebitoAutoDet>();
        }
    }

    public class clsEDebitoAutoDet
    {
        public int pCodigoMae { get; set; }
        public int pNroConsulta { get; set; }
        public string pServicioRef { get; set; }
        public int pJtsOidSaldo { get; set; }
        public string pEstado { get; set; }
        public string pDescError { get; set; }
    }

    public class enumEEDebitoAutoDetEstado
    {
        public readonly static string Error = "E";
        public readonly static string Pendiente = "PE";
        public readonly static string Pagado = "P";
    }
}
