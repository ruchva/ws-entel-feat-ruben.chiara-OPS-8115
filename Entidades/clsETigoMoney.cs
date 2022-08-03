using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class clsETigoMoney
    {
        public int pNroTransaccion { get; set; }
        public int pNroAsiento { get; set; }
        public DateTime pFechaProceso { get; set; }
        public string pHoraProceso { get; set; }
        public int pSucProceso { get; set; }
        public string pUsuProceso { get; set; }
        public long pNroTelefono { get; set; }
        public int pCodMoneda { get; set; }
        public decimal pMtoCarga { get; set; }
        public decimal pMtoComision { get; set; }
        public long pCodPersona { get; set; }
        public int pEstado { get; set; }
        public string pNomBilleteraConexion { get; set; }
        public string pNroPinConexion { get; set; }
        public string pNationalId { get; set; }
        public string pTipoTran { get; set; }
        public int pCodPlaza { get; set; }
        public string pDetails { get; set; }
        public string pNroSession { get; set; }
        public string pValorHash { get; set; }
        public string pTransIdLogin { get; set; }
        public string pTransId { get; set; }
        public string pResultWs { get; set; }
        public decimal pMtoComisionTigo { get; set; }
        public string pNroCupon { get; set; }
    }
}
