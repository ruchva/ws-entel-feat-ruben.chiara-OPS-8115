using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;
using System.Globalization;
using System.ServiceModel;
using System.Web.Services.Protocols;

namespace WcfGanadero
{
    /// <summary>
    /// Descripción breve de WsGanadero
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WsGanadero : System.Web.Services.WebService
    {
        #region ENTEL

        #region ENTEL --> TOPAZ

        [WebMethod]
        public string mtdBuscarClienteEntel(string codigoBusqueda, string codigoAcceso)
        {
            try
            {
                var entidad = new Negocio.clsNEntelWS().mtdBuscarCliente(codigoBusqueda, codigoAcceso);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pNroConsulta));
            }
            catch (Negocio.EntelException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pMsjError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        [WebMethod]
        public string mtdDetalleCliente(string pCodigoCuenta)
        {
            try
            {
                var entidad = new Negocio.clsNEntelWS().mtdDetalleCliente(pCodigoCuenta);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pNroConsulta));
            }
            catch (Negocio.EntelException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pMsjError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        [WebMethod]
        public string mtdConsultaPrepago(string pCodigoCuenta)
        {
            try
            {
                var entidad = new Negocio.clsNEntelWS().mtdConsultaPrepago(pCodigoCuenta);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pNroConsulta));
            }
            catch (Negocio.EntelException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pMsjError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        [WebMethod]
        public string mtdImpresion(string pLoteDosificacion, string pNumeroFactura, string tipoFactura)
        {
            try
            {
                var entidad = new Negocio.clsNEntelWS().mtdImpresion(pLoteDosificacion, pNumeroFactura, tipoFactura);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pNroConsulta));
            }
            catch (Negocio.EntelException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pMsjError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        [WebMethod]
        public string mtdObtenerReporte(string pFechaDesde, string pFechaHasta, string estado)
        {
            try
            {
                DateTime fechaDesde = DateTime.ParseExact(pFechaDesde, "ddMMyyyy", CultureInfo.InvariantCulture);
                DateTime fechaHasta = DateTime.ParseExact(pFechaHasta, "ddMMyyyy", CultureInfo.InvariantCulture);
                var entidad = new Negocio.clsNEntelWS().mtdObtenerReporte(fechaDesde, fechaHasta, estado);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pNroConsulta));
            }
            catch (Negocio.EntelException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pMsjError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        [WebMethod]
        public string mtdPagoPostPago(int pNroConsulta, int pCantPeriodos, string pDepto,
            string pCiudad, string pEntidad, string pAgencia, string pOperador, string pTipoCobranza,
            string pNombreNit, string pNroNit, string pTelefonoRef, string pTipoDocumento, string pComplemento, string pEmail, string pDireccionSucursal)
        {
            try
            {
                var entidad = new Negocio.clsNEntelWS().mtdPagoPostPago(pNroConsulta, pCantPeriodos, pDepto,
                                            pCiudad, pEntidad, pAgencia, pOperador, pTipoCobranza, "",
                                            pNombreNit,  pNroNit,  pTelefonoRef,  pTipoDocumento,  pComplemento,  pEmail,  pDireccionSucursal);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pNroConsulta));
            }
            catch (Negocio.EntelException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pMsjError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        [WebMethod]
        public string mtdRevertirPostPago(int pNroConsulta)
        {
            try
            {
                var entidad = new Negocio.clsNEntelWS().mtdRevertir(pNroConsulta);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pNroConsulta));
            }
            catch (Negocio.EntelException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pMsjError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        [WebMethod]
        public string mtdRevertir(int pNroConsulta)
        {
            try
            {
                var entidad = new Negocio.clsNEntelWS().mtdRevertir(pNroConsulta);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pNroConsulta));
            }
            catch (Negocio.EntelException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pMsjError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        [WebMethod]
        public string mtdAnular(int pNroConsulta)
        {
            try
            {
                var entidad = new Negocio.clsNEntelWS().mtdAnular(pNroConsulta);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pNroConsulta));
            }
            catch (Negocio.EntelException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pMsjError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        [WebMethod]
        public string mtdPagoPrePago(int pNroConsulta, decimal pMtoTotal, string pDepto,
            string pCiudad, string pEntidad, string pAgencia, string pOperador, string pTipoCobranza, string pNombreNit, string pNroNit,
            string pTelefonoRef,string pTipoDocumento,string pComplemento,string pEmail,string pDireccionSucursal)
        {
            try
            {
                var entidad = new Negocio.clsNEntelWS().mtdPagoPrePago(pNroConsulta, pMtoTotal, pDepto,
                                            pCiudad, pEntidad, pAgencia, pOperador, pTipoCobranza, pNombreNit, pNroNit, "", pTelefonoRef, pTipoDocumento, pComplemento, pEmail, pDireccionSucursal);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pNroConsulta));
            }
            catch (Negocio.EntelException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pMsjError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        #endregion

        #region ENTEL --> GANANET-GANAMOVIL

        [WebMethod]
        public Entidades.clsEEntelDetalle mtdObtenerDatosCliente(string codigoBusqueda, string codigoAcceso)
        {
            try
            {
                var entidad = new Negocio.clsNEntelWS().mtdBuscarCliente(codigoBusqueda, codigoAcceso);
                var detalle = entidad.pListDetalle
                                .Where(x => x.pLista == "DETALLE").FirstOrDefault();
                if (detalle == null)
                    throw new Exception("Entel no ha devuelto ningun resultado.");
                return detalle;
            }
            catch (Exception ex)
            {
                throw new SoapException(ex.Message, SoapException.ServerFaultCode);
            }
        }

        [WebMethod]
        public List<Entidades.clsEEntelDetalle> mtdObtenerServiciosCliente(string codigoBusqueda, string codigoAcceso)
        {
            try
            {
                var entidad = new Negocio.clsNEntelWS().mtdBuscarCliente(codigoBusqueda, codigoAcceso);
                var detalle = new List<Entidades.clsEEntelDetalle>();
                var servicios = entidad.pListDetalle
                                .Where(x => x.pLista == "SERVICIO").ToList();
                var formasPago = entidad.pListDetalle
                                .Where(x => x.pLista == "FORMAS").ToList();
                var cuenta = entidad.pListDetalle
                                .Where(x => x.pLista == "DETALLE").FirstOrDefault();
                foreach (Entidades.clsEEntelDetalle item in servicios)
                {
                    if (item.pServicio == "POS")
                    {
                        var servicio = new Entidades.clsEEntelDetalle();
                        servicio.pNroConsulta = item.pNroConsulta;
                        servicio.pServicio = item.pServicio;
                        servicio.pCuenta = cuenta == null ? "" : cuenta.pCuenta;
                        servicio.pFormaPago = item.pServicio + "|" + "TC_POST";
                        servicio.pDesFormaPago = "";
                        detalle.Add(servicio);
                        continue;
                    }
                    foreach (Entidades.clsEEntelDetalle formaPago in formasPago)
                    {
                        if (item.pServicio == "PRE" && formaPago.pFormaPago == "TC_POST")
                            continue;

                        var servicio = new Entidades.clsEEntelDetalle();
                        servicio.pNroConsulta = item.pNroConsulta;
                        servicio.pServicio = item.pServicio;
                        servicio.pCuenta = cuenta == null ? "" : cuenta.pCuenta;
                        servicio.pFormaPago = item.pServicio + "|" + formaPago.pFormaPago;
                        servicio.pDesFormaPago = formaPago.pDesFormaPago;
                        detalle.Add(servicio);
                    }
                }

                if (detalle == null)
                    throw new Exception("Entel no ha devuelto ningun resultado.");
                return detalle;
            }
            catch (Exception ex)
            {
                throw new SoapException(ex.Message, SoapException.ServerFaultCode);
            }
        }

        [WebMethod]
        public List<Entidades.clsEEntelDetalle> mtdObtenerDeudasPendientes(string pCodigoCuenta, string pTipoPago)
        {
            try
            {
                if (pTipoPago == "POS")
                {
                    var entidad = new Negocio.clsNEntelWS().mtdDetalleCliente(pCodigoCuenta);
                    var clsEEntelDetalleListPagar = new List<Entidades.clsEEntelDetalle>();
                    entidad.pListDetalle = entidad.pListDetalle.OrderBy(x => x.pPeriodo).ToList();
                    Entidades.clsEEntelDetalle anterior = null;
                    foreach (var detalle in entidad.pListDetalle)
                    {
                        if (anterior != null && detalle.pPeriodo == anterior.pPeriodo)
                        {
                            anterior.pMtoPagar += detalle.pMtoPagar;
                        }
                        else
                        {
                            anterior = detalle;
                            clsEEntelDetalleListPagar.Add(anterior);
                        }
                    }
                    return clsEEntelDetalleListPagar;
                }
                else if (pTipoPago == "PRE")
                {
                    var entidad = new Negocio.clsNEntelWS().mtdConsultaPrepago(pCodigoCuenta);
                    return entidad.pListDetalle;
                }
                else
                {
                    throw new Exception("No se reconoce el tipo de pago");
                }
            }
            catch (Exception ex)
            {
                throw new SoapException(ex.Message, SoapException.ServerFaultCode);
            }
        }

        [WebMethod]
        public Entidades.clsEEntel mtdPagoPrePos(int pNroConsulta, string pTipoPago, decimal pMtoTotal, string pDepto,
            string pCiudad, string pEntidad, string pAgencia, string pOperador, string pTipoCobranza, string pNombreNit, string pNroNit, string pNroTransaccion,
            string pTelefonoRef, string pTipoDocumento, string pComplemento, string pEmail, string pDireccionSucursal)
        {
            try
            {
                //try
                //{
                //    if (pOperador == "JBK")
                //    {
                //        if (esGanaNet(pNroTransaccion))
                //        {
                //            pDepto = "10";
                //            pCiudad = "209";
                //            pAgencia = "1";
                //            pOperador = "1";
                //        }
                //        else
                //        {
                //            pDepto = "10";
                //            pCiudad = "142";
                //            pAgencia = "1";
                //            pOperador = "1";
                //        }
                //    }
                //}
                //catch (Exception ex) { Negocio.clsNUtils.escribirLogError(ex); }

                if (pTipoPago == "POS")
                {
                    var entidad = new Negocio.clsNEntelWS().mtdPagoPostPago(pNroConsulta, 1, pDepto,
                                            pCiudad, pEntidad, pAgencia, pOperador, pTipoCobranza, pNroTransaccion, pNombreNit, pNroNit, pTelefonoRef, pTipoDocumento, pComplemento, pEmail, pDireccionSucursal);
                    return entidad;
                }
                else if (pTipoPago == "PRE")
                {
                    var entidad = new Negocio.clsNEntelWS().mtdPagoPrePago(pNroConsulta, pMtoTotal, pDepto,
                                            pCiudad, pEntidad, pAgencia, pOperador, pTipoCobranza, pNombreNit, pNroNit, pNroTransaccion, pTelefonoRef, pTipoDocumento, pComplemento, pEmail,pDireccionSucursal);
                    return entidad;
                }
                else
                {
                    throw new Exception("No se reconoce el tipo de pago");
                }
            }
            catch (Exception ex)
            {
                throw new SoapException(ex.Message, SoapException.ServerFaultCode);
            }
        }

        #endregion

        #region DEBITO AUTO ENTEL --> TOPAZ

        [WebMethod]
        public string mtdDebitoAutoEntel(int pCodConvenio, int pCodPlaza, string pUsuario)
        {
            try
            {
                var entidad = new Negocio.clsNDebitoAuto().mtdDebitoEntel(pCodConvenio, pCodPlaza, pUsuario);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pCodigo));
            }
            catch (Negocio.EntelException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pMsjError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }
        [WebMethod]
        public string mtdDebitoAutoEntelXUno(int pCodigo,int pCodConvenio, int pCodPlaza, string pUsuario,string pServicioReferencia, int pJtsOid)
        {
            try
            {
                var entidad = new Negocio.clsNDebitoAuto().mtdDebitoEntelXUno( pCodigo,pCodConvenio, pCodPlaza, pUsuario, pServicioReferencia,  pJtsOid);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pCodigo));
            }
            catch (Negocio.EntelException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pMsjError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }
        [WebMethod]
        public string mtdAplicarDebitoEntel(int pCodigoMae, string pUsuario, string pSucursal)
        {
            try
            {
                var entidad = new Negocio.clsNDebitoAuto().mtdAplicarDebitoEntel(pCodigoMae, pUsuario, pSucursal);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pCodigo));
            }
            catch (Negocio.EntelException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pMsjError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }
        [WebMethod]
        public string mtdAplicarDebitoEntelXUno(int pCodigoMae, string pUsuario, string pSucursal, int pNroConsulta)
        {
            try
            {
                var entidad = new Negocio.clsNDebitoAuto().mtdAplicarDebitoEntelxUno(pCodigoMae, pUsuario, pSucursal, pNroConsulta);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pCodigo));
            }
            catch (Negocio.EntelException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pMsjError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }
        #endregion

        #endregion

        #region UTILIDADES

        public static string ConvertirAXml<T>(T pObjeto)
        {
            return Negocio.clsNUtils.ConvertirAXml(pObjeto);
        }

        [WebMethod]
        public bool esGanaNet(string nroTransaccion)
        {

            var bd = new Datos.clsOracleConexion();
            var sql = @"select g.CANAL
                            from GANANET.GNT_PAGOSERVICIOSMAESTRO p, gnt_autorizaciones g
                            where p.ebmsrtdes = {0} 
                            AND g.ebautntra = ebmsrntra AND g.ebautcper = ebmsrcage";
            sql = string.Format(sql, nroTransaccion);
            var dataSet = bd.mtdEjecutarConsulta(sql);
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                throw new Exception("El nro de transaccion no existe.");

            return dataSet.Tables[0].Rows[0][0].ToString() == "0";

        }

        #endregion
    }
}
