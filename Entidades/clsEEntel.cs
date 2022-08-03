using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class clsEEntel
    {
        public int pNroConsulta { get; set; }
        public string pNomMetodo { get; set; }
        public string pCodBusqueda { get; set; }
        public string pCodAcceso { get; set; }
        public string pEntCobranza { get; set; }
        public string pCuenta { get; set; }
        public DateTime pFecha { get; set; }
        public string pHora { get; set; }
        public string pCodError { get; set; }
        public string pMsjError { get; set; }
        public List<clsEEntelDetalle> pListDetalle { get; set; }
        /// <summary>
        /// CAmbios para Entel 
        /// </summary>
        /// 
        public string pComplemento { get; set; }
        public string pEmail { get; set; }
        public string pTelefonoRef { get; set; }
        public string pTipoDocumento { get; set; }
        public string pDireccionSucursal { get; set; }

        /// <summary>
        /// entel parametros demo
        /// </summary>
        public string pNit { get; set; }
        public string pRazonSocial { get; set; }


        public clsEEntel()
        {
            this.pListDetalle = new List<clsEEntelDetalle>();
        }

        public override string ToString()
        {
            string str = "clsEEntel--> pNroConsulta: " + pNroConsulta;
            str += ", pNomMetodo: " + pNomMetodo ?? "";
            str += ", pCodBusqueda: " + pCodBusqueda ?? "";
            str += ", pCodAcceso: " + pCodAcceso ?? "";
            str += ", pEntCobranza: " + pEntCobranza ?? "";
            str += ", pCuenta: " + pCuenta ?? "";
            str += ", pFecha: " + pFecha.ToString();
            str += ", pHora: " + pHora ?? "";
            str += ", pCodError: " + pCodError ?? "";
            str += ", pMsjError: " + pMsjError ?? "";
            ///
            str += ", pNit: " + pNit ?? "";
            str += ", pRazonSocial: " + pRazonSocial ?? "";
            return str;
        }
    }

    public class clsEEntelDetalle
    {
        public int pNroConsulta { get; set; }
        public string pNomMetodo { get; set; }
        public string pLista { get; set; }
        public string pTipoCuenta { get; set; }
        public string pNombre { get; set; }
        public string pCuenta { get; set; }
        public decimal pMtoDeuda { get; set; }
        public string pServicio { get; set; }
        public string pFormaPago { get; set; }
        public string pDesFormaPago { get; set; }
        public string pDesServicio { get; set; }
        public string pPeriodo { get; set; }
        public string pLoteDosificacion { get; set; }
        public string pNumeroRenta { get; set; }
        public decimal pMtoPagar { get; set; }
        public string pTipoFactura { get; set; }
        public string pRazonSocial { get; set; }
        public string pNit { get; set; }
        public string pFactura { get; set; }
        public decimal pSaldo { get; set; }
        public decimal pMtoMinimo { get; set; }
        public string pDepto { get; set; }
        public string pCiudad { get; set; }
        public string pEntidad { get; set; }
        public string pAgencia { get; set; }
        public string pOperador { get; set; }
        public string pTipoCobranza { get; set; }
        public DateTime pFecha { get; set; }
        public string pHora { get; set; }
        /// <summary>
        /// Cambios Nuevos parametros ENTEL
        /// </summary>
        public string pTelefonoRef { get; set; }
        public string pTipoDocumento { get; set; }
        public string pComplemento { get; set; }
        public string pEmail { get; set; }
        public string pDireccionSucursal { get; set; }
                
        public string pServicioDes
        {
            get
            {
                if (pServicio == null) return null;
                switch (pServicio)
                {
                    case "PRE": return "Recarga" + (pDesFormaPago == null ? "" : " - " + pDesFormaPago);
                    case "POS": return "PostPago";
                    default: return "";
                }
            }
            set
            {

            }
        }

        public override string ToString()
        {
            string str = "clsEEntelDetalle--> pNroConsulta: " + pNroConsulta;
            str += ", pNomMetodo: " + pNomMetodo ?? "";
            str += ", pLista: " + pLista ?? "";
            str += ", pTipoCuenta: " + pTipoCuenta ?? "";
            str += ", pNombre: " + pNombre ?? "";
            str += ", pCuenta: " + pCuenta ?? "";
            str += ", pMtoDeuda: " + pMtoDeuda ?? "";
            str += ", pServicio: " + pServicio ?? "";
            str += ", pFormaPago: " + pFormaPago ?? "";
            str += ", pDesFormaPago: " + pDesFormaPago ?? "";
            str += ", pDesServicio: " + pDesServicio ?? "";
            str += ", pPeriodo: " + pPeriodo ?? "";
            str += ", pLoteDosificacion: " + pLoteDosificacion ?? "";
            str += ", pNumeroRenta: " + pNumeroRenta ?? "";
            str += ", pMtoPagar: " + pMtoPagar ?? "";
            str += ", pTipoFactura: " + pTipoFactura ?? "";
            str += ", pRazonSocial: " + pRazonSocial ?? "";
            str += ", pNit: " + pNit ?? "";
            str += ", pFactura: " + pFactura ?? "";
            str += ", pSaldo: " + pSaldo ?? "";
            str += ", pMtoMinimo: " + pMtoMinimo ?? "";
            str += ", pDepto: " + pDepto ?? "";
            str += ", pCiudad: " + pCiudad ?? "";
            str += ", pEntidad: " + pEntidad ?? "";
            str += ", pAgencia: " + pAgencia ?? "";
            str += ", pOperador: " + pOperador ?? "";
            str += ", pTipoCobranza: " + pTipoCobranza ?? "";
            str += ", pFecha: " + pFecha ?? "";
            str += ", pHora: " + pHora ?? "";
            /// <summary>
            /// Cambios Nuevos parametros ENTEL
            /// </summary>
            str += ", pTelefonoRef: " + pTelefonoRef ?? "";
            str += ", pTipoDocumento: " + pTipoDocumento ?? "";
            str += ", pComplemento: " + pComplemento ?? "";
            str += ", pEmail: " + pEmail ?? "";
            str += ", pDireccionSucursal: " + pDireccionSucursal ?? "";
            ///
            str += ", pNit: " + pNit ?? "";
            str += ", pRazonSocial: " + pRazonSocial ?? "";


            return str;
        }

    }

    public class clsEEntelReporte
    {
        public int pNroConsulta { get; set; }
        public string pAgencia { get; set; }
        public string pCiudad { get; set; }
        public string pCuenta { get; set; }
        public string pDepartamento { get; set; }
        public string pDosificacionPrepago { get; set; }
        public string pEstado { get; set; }
        public DateTime pFecha { get; set; }
        public string pHora { get; set; }
        public decimal pImporte { get; set; }
        public string pLote { get; set; }
        public string pNit { get; set; }
        public string pNumeroRenta { get; set; }
        public string pNumeroRentaPrepago { get; set; }
        public string pOperador { get; set; }
        public string pPeriodo { get; set; }
        public string pRazonSocial { get; set; }
        public string pTipoPago { get; set; }

        public override string ToString()
        {
            var str = "clsEEntelReporte--> pNroConsulta: " + pNroConsulta;
            str += ", pAgencia: " + pAgencia ?? "";
            str += ", pCiudad: " + pCiudad ?? "";
            str += ", pCuenta: " + pCuenta ?? "";
            str += ", pDepartamento: " + pDepartamento ?? "";
            str += ", pDosificacionPrepago: " + pDosificacionPrepago ?? "";
            str += ", pEstado: " + pEstado ?? "";
            str += ", pFecha: " + pFecha ?? "";
            str += ", pHora: " + pHora ?? "";
            str += ", pImporte: " + pImporte ?? "";
            str += ", pLote: " + pLote ?? "";
            str += ", pNit: " + pNit ?? "";
            str += ", pNumeroRenta: " + pNumeroRenta ?? "";
            str += ", pNumeroRentaPrepago: " + pNumeroRentaPrepago ?? "";
            str += ", pOperador: " + pOperador ?? "";
            str += ", pPeriodo: " + pPeriodo ?? "";
            str += ", pRazonSocial: " + pRazonSocial ?? "";
            str += ", pTipoPago: " + pTipoPago ?? "";
            return str;
        }
    }

    public class clsEEntelImpresion
    {
        public int pNroConsulta { get; set; }
        public int pOrden { get; set; }
        public int pOrden2 { get; set; }
        public int pNroPag { get; set; }
        public string pLoteDosificacion { get; set; }
        public string pNumeroRenta { get; set; }
        public string pTipoFactura { get; set; }
        public string pDetalle { get; set; }

        public override string ToString()
        {
            var str = "clsEEntelImpresion--> pNroConsulta: " + pNroConsulta;
            str += ", pOrden: " + pOrden ?? "";
            str += ", pOrden2: " + pOrden2 ?? "";
            str += ", pNroPag: " + pNroPag ?? "";
            str += ", pLoteDosificacion: " + pLoteDosificacion ?? "";
            str += ", pNumeroRenta: " + pNumeroRenta ?? "";
            str += ", pTipoFactura: " + pTipoFactura ?? "";
            str += ", pDetalle: " + pDetalle ?? "";
            return str;
        }
    }

    public class clsEEntelPago
    {
        public string pLoteDosificacion { get; set; }
        public string pNumeroRenta { get; set; }
        public string pMontoTotal { get; set; }
        public string pDepartamento { get; set; }
        public string pCiudad { get; set; }
        public string pEntidad { get; set; }
        public string pAgencia { get; set; }
        public string pOperador { get; set; }
        public string pFecha { get; set; }
        public string pHora { get; set; }
        public string pFormaPago { get; set; }
        public string pTransaccion { get; set; }
        public string pEntidadCobranza { get; set; }
        public string pTipoCobranza { get; set; }
        public string pCodigoCuenta { get; set; }
        public string pNombreFactura { get; set; }
        public string pNitFactura { get; set; }
        public string pObservacion { get; set; }
        /// <summary>
        /// Cambios Nuevos parametros ENTEL
        /// </summary>
        public string pTelefonoRef { get; set; }
        public string pTipoDocumento { get; set; }
        public string pComplemento { get; set; }
        public string pEmail { get; set; }
    }

    public class enumEEntelDetalleMetodo
    {
        public readonly static string DetalleCliente = "DETALLECLIENTE";
    }

    public class enumEEntelDetalleTipo
    {
        public readonly static string Detalle = "DETALLE";
        public readonly static string Servicio = "SERVICIO";
        public readonly static string FormaPago = "FORMAS";
        public readonly static string Resultado = "RESULTADO";
    }
}
