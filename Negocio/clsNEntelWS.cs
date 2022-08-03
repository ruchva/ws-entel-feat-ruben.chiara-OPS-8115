using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Configuration;
using System.Globalization;
using Newtonsoft.Json;
using System.ServiceModel;

namespace Negocio
{
    public class clsNEntelWS
    {
        private WcfEntel.ServiciosPostEntelClient wcfEntel;
        private string gUsuario;
        private string gPassword;
        public string gEntidadFinanciera;
        public string gEntidadCobranza;
        private string gFormaPago;
        private string gTipoFactura;
        private CultureInfo gCultureInfo;
        private string gFechaFormato;
        private string gHoraFormato;
        private string gErrorServicioEntel = "Error de conexion con Entel, intente nuevamente.";

        public clsNEntelWS()
        {
            wcfEntel = new WcfEntel.ServiciosPostEntelClient();
            gUsuario = ConfigurationManager.AppSettings["Entel_Usuario"];
            gPassword = ConfigurationManager.AppSettings["Entel_Password"];
            gEntidadFinanciera = ConfigurationManager.AppSettings["Entel_EntidadFinaciera"];
            gEntidadCobranza = ConfigurationManager.AppSettings["Entel_EntidadCobranza"];
            gFormaPago = ConfigurationManager.AppSettings["Entel_FormaPago"];
            gTipoFactura = ConfigurationManager.AppSettings["Entel_TipoFactura"];
            gCultureInfo = new CultureInfo("en-US");
            gFechaFormato = "yyyyMMdd";
            gHoraFormato = "HHmmss";
        }

        /// <summary>
        /// Este método nos permite consultar un cliente.
        /// </summary>
        /// <param name="codigoBusqueda">Código de cliente = 1; Código de cuenta = 2; Número de teléfono = 3; </param>
        /// <param name="codigoAcceso">Valor referente al tipo de búsqueda a realizar</param>
        /// <returns><see cref="Entidades.clsEEntel"/></returns>
        public Entidades.clsEEntel mtdBuscarCliente(string pCodigoBusqueda, string pCodigoAcceso)
        {
            try
            {
                var nombreMetodo = "BUSCARCLIENTE";
                var source = "clsNEntelWS.mtdBuscarCliente";
                var sourceEntel = "WcfEntel.ServiciosPostEntelClient.BuscarCliente";

                var datosEntrada = mtdCrearDatosEntrada();
                datosEntrada.codigoBusqueda = pCodigoBusqueda;
                datosEntrada.codigoAcceso = pCodigoAcceso;

                clsNUtils.escribirLogInfo(datosEntrada, sourceEntel + ".DatosEntrada");
                System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidCert);
                var respuesta = this.wcfEntel.BuscarCliente(datosEntrada);
                clsNUtils.escribirLogInfo(respuesta, sourceEntel + ".Respuesta");

                var clsEEntel = mtdCrearClsEEntel(nombreMetodo, pCodigoBusqueda, pCodigoAcceso,
                    "", respuesta.flagError, respuesta.mensaje);

                clsNUtils.escribirLogInfo(clsEEntel.ToString(), source);

                clsEEntel = mtdInsertarConsulta(clsEEntel);

                if (!string.IsNullOrWhiteSpace(respuesta.flagError) && respuesta.flagError == "S")
                    throw new EntelException(respuesta.flagError, respuesta.mensaje);

                if (respuesta.detalle != null)
                    foreach (var item in respuesta.detalle)
                    {
                        var clsEEntelDetalle = mtdCrearClsEEntelDet(clsEEntel.pNroConsulta, nombreMetodo);
                        clsEEntelDetalle.pNombre = item.nombre;
                        clsEEntelDetalle.pCuenta = item.cuenta;
                        clsEEntelDetalle.pMtoDeuda = Convert.ToDecimal(item.montoDeuda, gCultureInfo);
                        clsEEntelDetalle.pTipoCuenta = item.tipoCuenta;
                        clsEEntelDetalle.pLista = "DETALLE";

                        clsNUtils.escribirLogInfo(clsEEntelDetalle.ToString(), source);
                        mtdInsertarDetalle(clsEEntelDetalle);
                        clsEEntel.pListDetalle.Add(clsEEntelDetalle);
                    }

                if (respuesta.servicios != null)
                    foreach (var item in respuesta.servicios)
                    {
                        var clsEEntelDetalle = mtdCrearClsEEntelDet(clsEEntel.pNroConsulta, nombreMetodo);
                        clsEEntelDetalle.pServicio = item.servicio;
                        clsEEntelDetalle.pLista = "SERVICIO";

                        clsNUtils.escribirLogInfo(clsEEntelDetalle.ToString(), source);
                        mtdInsertarDetalle(clsEEntelDetalle);
                        clsEEntel.pListDetalle.Add(clsEEntelDetalle);
                    }

                if (respuesta.formasPago != null)
                    foreach (var item in respuesta.formasPago)
                    {
                        var clsEEntelDetalle = mtdCrearClsEEntelDet(clsEEntel.pNroConsulta, nombreMetodo);
                        clsEEntelDetalle.pFormaPago = item.formaPago;
                        clsEEntelDetalle.pDesFormaPago = item.formaPagoDescripcion;
                        clsEEntelDetalle.pServicio = item.servicio;
                        clsEEntelDetalle.pDesServicio = item.servicioDescripcion;
                        clsEEntelDetalle.pLista = "FORMAS";

                        clsNUtils.escribirLogInfo(clsEEntelDetalle.ToString(), source);
                        mtdInsertarDetalle(clsEEntelDetalle);
                        clsEEntel.pListDetalle.Add(clsEEntelDetalle);

                        if (!string.IsNullOrWhiteSpace(item.servicio))
                        {
                            var clsEEntelDetalle2 = mtdCrearClsEEntelDet(clsEEntel.pNroConsulta, nombreMetodo);
                            clsEEntelDetalle2.pFormaPago = item.servicio;
                            clsEEntelDetalle2.pDesFormaPago = item.servicioDescripcion;
                            clsEEntelDetalle2.pLista = "FORMAS";
                            clsNUtils.escribirLogInfo(clsEEntelDetalle2.ToString(), source);
                            mtdInsertarDetalle(clsEEntelDetalle2);
                            clsEEntel.pListDetalle.Add(clsEEntelDetalle2);
                        }
                    }

                return clsEEntel;
            }
            catch (EndpointNotFoundException ex)
            {
                clsNUtils.escribirLogError(ex, gErrorServicioEntel);
                throw new Exception(gErrorServicioEntel);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Devuelve un conjunto de facturas pendientes de pagar
        /// </summary>
        /// <param name="codigoCuenta">Dato capturado por método Buscar Cliente</param>
        /// <returns><see cref="Entidades.clsEEntel"/></returns>
        public Entidades.clsEEntel mtdDetalleCliente(string pCodigoCuenta)
        {
            try
            {
                var nombreMetodo = "DETALLECLIENTE";
                var source = "clsNEntelWS.mtdDetalleCliente";
                var sourceEntel = "WcfEntel.ServiciosPostEntelClient.DetalleCliente";

                var datosEntrada = mtdCrearDatosEntrada();
                datosEntrada.cuenta = pCodigoCuenta;
                clsNUtils.escribirLogInfo(datosEntrada, sourceEntel + ".DatosEntrada");
                System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidCert);
                var respuesta = this.wcfEntel.DetalleCliente(datosEntrada);
                clsNUtils.escribirLogInfo(respuesta, sourceEntel + ".Respuesta");

                var clsEEntel = mtdCrearClsEEntel(nombreMetodo, "", "", pCodigoCuenta, respuesta.flagError, respuesta.mensaje, null, null, "", "", "", "", (respuesta.flagError == "S" ? "" : respuesta.detalle[0].razonSocial), (respuesta.flagError == "S" ? "" : respuesta.detalle[0].nit));

                clsNUtils.escribirLogInfo(clsEEntel.ToString(), source);

                clsEEntel = mtdInsertarConsulta(clsEEntel);

                if (!string.IsNullOrWhiteSpace(respuesta.flagError) && respuesta.flagError == "S")
                    throw new EntelException(respuesta.flagError, respuesta.mensaje);

                if (respuesta.detalle != null)
                    foreach (var item in respuesta.detalle)
                    {
                        var clsEEntelDetalle = mtdCrearClsEEntelDet(clsEEntel.pNroConsulta, nombreMetodo);
                        clsEEntelDetalle.pPeriodo = item.periodo;
                        clsEEntelDetalle.pLoteDosificacion = item.loteDosificacion;
                        clsEEntelDetalle.pNumeroRenta = item.numeroRenta;
                        clsEEntelDetalle.pMtoPagar = Convert.ToDecimal(item.montoAPagar, gCultureInfo);
                        clsEEntelDetalle.pTipoFactura = item.tipoFactura;
                        clsEEntelDetalle.pRazonSocial = item.razonSocial;
                        clsEEntelDetalle.pNit = item.nit;
                        clsEEntelDetalle.pFactura = item.factura;
                        clsEEntelDetalle.pLista = "DETALLE";

                        clsNUtils.escribirLogInfo(clsEEntelDetalle.ToString(), source);
                        mtdInsertarDetalle(clsEEntelDetalle);
                        clsEEntel.pListDetalle.Add(clsEEntelDetalle);
                    }
                return clsEEntel;
            }
            catch (EndpointNotFoundException ex)
            {
                clsNUtils.escribirLogError(ex, gErrorServicioEntel);
                throw new Exception(gErrorServicioEntel);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }

        }

        /// <summary>
        /// Este método retorna el monto mínimo de compra por los servicios prepago
        /// </summary>
        /// <param name="codigoCuenta">Número de teléfono prepago</param>
        /// <returns><see cref="Entidades.clsEEntel"/></returns>
        public Entidades.clsEEntel mtdConsultaPrepago(string pCodigoCuenta)
        {
            try
            {
                var nombreMetodo = "CONSULTAPREPAGO";
                var source = "clsNEntelWS.mtdConsultaPrepago";
                var sourceEntel = "WcfEntel.ServiciosPostEntelClient.ConsultaPrepago";
                var fecha = DateTime.Now;
                var datosEntrada = mtdCrearDatosEntrada();
                datosEntrada.cuenta = pCodigoCuenta;
                datosEntrada.fecha = fecha.ToString(this.gFechaFormato);
                datosEntrada.hora = fecha.ToString(this.gHoraFormato);
                clsNUtils.escribirLogInfo(datosEntrada, sourceEntel + ".DatosEntrada");
                System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidCert);
                var respuesta = this.wcfEntel.ConsultaPrepago(datosEntrada);
                clsNUtils.escribirLogInfo(respuesta, sourceEntel + ".Respuesta");

                var clsEEntel = mtdCrearClsEEntel(nombreMetodo, "", "",
                    pCodigoCuenta, respuesta.flagError, respuesta.mensaje, fecha, fecha,
                    respuesta.telefonoRef, respuesta.tipoDocumento, respuesta.complemento, respuesta.email, respuesta.razon,respuesta.nit);

                clsNUtils.escribirLogInfo(clsEEntel.ToString(), source);

                clsEEntel = mtdInsertarConsulta(clsEEntel);

                if (!string.IsNullOrWhiteSpace(respuesta.flagError) && respuesta.flagError == "S")
                    throw new EntelException(respuesta.flagError, respuesta.mensaje);

                if (respuesta.detalle != null)
                    foreach (var item in respuesta.detalle)
                    {
                        var clsEEntelDetalle = mtdCrearClsEEntelDet(clsEEntel.pNroConsulta, nombreMetodo);
                        clsEEntelDetalle.pSaldo = Convert.ToDecimal(item.saldo, gCultureInfo);
                        clsEEntelDetalle.pMtoMinimo = Convert.ToDecimal(item.montoMinimo, gCultureInfo);
                        clsEEntelDetalle.pLista = "DETALLE";

                        clsNUtils.escribirLogInfo(clsEEntelDetalle.ToString(), source);
                        mtdInsertarDetalle(clsEEntelDetalle);
                        clsEEntel.pListDetalle.Add(clsEEntelDetalle);
                    }

                return clsEEntel;
            }
            catch (EndpointNotFoundException ex)
            {
                clsNUtils.escribirLogError(ex, gErrorServicioEntel);
                throw new Exception(gErrorServicioEntel);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Este método nos permite realizar transacciones por concepto de servicios postpago
        /// </summary>
        /// <param name="pTipoTransaccion">P(Pago), R(Reversión), A(Anulación)</param>
        /// <returns><see cref="Entidades.clsEEntel"/></returns>
        private List<Entidades.clsEEntelDetalle> mtdPagoVenta(List<WcfEntel.datosEntrada> pListaDatosEntrada,
            List<Entidades.clsEEntelDetalle> pListaClsEEntelDetalle, string nombreMetodo, string pSource)
        {
            try
            {
                var source = pSource;
                var sourceEntel = "WcfEntel.ServiciosPostEntelClient.PagoVenta";

                clsNUtils.escribirLogInfo(pListaDatosEntrada, sourceEntel + ".DatosEntrada");
                System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidCert);
                var respuesta = this.wcfEntel.PagoVenta(pListaDatosEntrada.ToArray());

                clsNUtils.escribirLogInfo(respuesta, sourceEntel + ".Respuesta");

                if (respuesta == null)
                    throw new EntelException("S", "Error Entel no ha devuelto ningun resultado");
                foreach (var result in respuesta)
                {
                    if (!string.IsNullOrWhiteSpace(result.flagError) && result.flagError == "S")
                        throw new EntelException(result.flagError, result.mensaje);
                }
                var listaRetorno = new List<Entidades.clsEEntelDetalle>();
                int i = 0;
                foreach (var clsEEntelPago in pListaClsEEntelDetalle)
                {
                    var resp = respuesta[i];

                    var clsEEntelDetalle = mtdCrearClsEEntelDet(clsEEntelPago.pNroConsulta, nombreMetodo);
                    clsEEntelDetalle.pLista = "RESULTADO";
                    clsEEntelDetalle.pPeriodo = clsEEntelPago.pPeriodo;
                    clsEEntelDetalle.pLoteDosificacion = string.IsNullOrWhiteSpace(resp.dosificacionPrepago) ? clsEEntelPago.pLoteDosificacion : resp.dosificacionPrepago;
                    clsEEntelDetalle.pNumeroRenta = string.IsNullOrWhiteSpace(resp.nuemroRentaPrepago) ? clsEEntelPago.pNumeroRenta : resp.nuemroRentaPrepago;
                    clsEEntelDetalle.pMtoPagar = Convert.ToDecimal(clsEEntelPago.pMtoPagar, gCultureInfo);
                    clsEEntelDetalle.pTipoFactura = this.gTipoFactura;
                    clsEEntelDetalle.pRazonSocial = clsEEntelPago.pRazonSocial;
                    clsEEntelDetalle.pNit = clsEEntelPago.pNit;
                    clsEEntelDetalle.pFactura = clsEEntelPago.pFactura;
                    clsEEntelDetalle.pDepto = clsEEntelPago.pDepto;
                    clsEEntelDetalle.pCiudad = clsEEntelPago.pCiudad;
                    // clsEEntelDetalle.pEntidad = clsEEntelPago.pEntidad; --> Siempre va ser el mismo valor
                    clsEEntelDetalle.pAgencia = clsEEntelPago.pAgencia;
                    clsEEntelDetalle.pOperador = clsEEntelPago.pOperador;
                    clsEEntelDetalle.pTipoCobranza = clsEEntelPago.pTipoCobranza;
                    clsEEntelDetalle.pFecha = clsEEntelPago.pFecha;
                    clsEEntelDetalle.pHora = clsEEntelPago.pHora;

                    clsEEntelDetalle.pNombre = clsEEntelPago.pNombre; // Nro Transaccion(GANANET-GANAMOVIL)

                    clsNUtils.escribirLogInfo(clsEEntelDetalle.ToString(), source);
                    mtdInsertarDetalle(clsEEntelDetalle);
                    listaRetorno.Add(clsEEntelDetalle);
                }
                return listaRetorno;
            }
            catch (EndpointNotFoundException ex)
            {
                clsNUtils.escribirLogError(ex, gErrorServicioEntel);
                throw new Exception(gErrorServicioEntel);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Este método nos permite Pagar un servicio postpago
        /// </summary>
        /// <returns><see cref="Entidades.clsEEntel"/></returns>
        public Entidades.clsEEntel mtdPagoPostPago(int pNroConsulta, int pCantPeriodos, string pDepto,
            string pCiudad, string pEntidad, string pAgencia, string pOperador, string pTipoCobranza, string pNroTransaccion,
            string pNombreNit, string pNroNit, string pTelefonoRef, string pTipoDocumento, string pComplemento, string pEmail, string pDireccionSucursal)
        {
            try
            {
                var nombreMetodo = "PAGOVENTA";
                var source = "clsNEntelWS.mtdPagoPostPago";

                clsNUtils.escribirLogInfo("pNroConsulta: " + pNroConsulta + ", pCantPeriodos: " + pCantPeriodos, source);

                var clsEEntel = mtdObtenerConsulta(pNroConsulta);
                var clsEEntelDetalleList = mtdObtenerDetalle(pNroConsulta);

                if (clsEEntelDetalleList.Count == 0)
                    throw new Exception("Error la consulta no tiene detalle.");

                var dtDeudasPorPeriodo = mtdObtenerDeudasPorPeriodo(pNroConsulta);

                var listaDatosEntrada = new List<WcfEntel.datosEntrada>();
                var fechaReg = DateTime.Now;
                
                var next = 0;

                var clsEEntelDetalleListPagar = new List<Entidades.clsEEntelDetalle>();

                foreach (DataRow dataRow in dtDeudasPorPeriodo.Rows)
                {
                    if (next == pCantPeriodos)
                        break;

                    var periodo = dataRow["PERIODO"].ToString();
                    var clsEEntelDetList = clsEEntelDetalleList
                        .Where(x => x.pPeriodo == periodo && 
                            x.pNomMetodo == "DETALLECLIENTE" 
                            && x.pLista == "DETALLE");

                    foreach (var detalle in clsEEntelDetList)
                    {
                        detalle.pDepto = pDepto;
                        detalle.pCiudad = pCiudad;
                        detalle.pEntidad = pEntidad;
                        detalle.pAgencia = pAgencia;
                        detalle.pOperador = pOperador;
                        detalle.pTipoCobranza = pTipoCobranza;
                        detalle.pFecha = fechaReg;
                        detalle.pHora = fechaReg.ToString(this.gHoraFormato);
                        detalle.pNombre = pNroTransaccion;
                        clsEEntelDetalleListPagar.Add(detalle);

                        var datosEntrada = mtdCrearDatosEntrada(detalle);
                        datosEntrada.transaccion = "P";
                        datosEntrada.cuenta = clsEEntel.pCuenta;
                        datosEntrada.observacion = "";

                        listaDatosEntrada.Add(datosEntrada);
                    }
                    next++;
                }
                mtdPagoVenta(listaDatosEntrada, clsEEntelDetalleListPagar, nombreMetodo, source);

                return clsEEntel;
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Este método nos permite Revertir un pago
        /// </summary>
        /// <returns><see cref="Entidades.clsEEntel"/></returns>
        public Entidades.clsEEntel mtdRevertir(int pNroConsulta)
        {
            try
            {
                var nombreMetodo = "REVERTIR";
                var source = "clsNEntelWS.mtdRevertirPostPago";

                clsNUtils.escribirLogInfo("pNroConsulta: " + pNroConsulta, source);

                var clsEEntel = mtdObtenerConsulta(pNroConsulta);
                var clsEEntelDetalleList = mtdObtenerDetalle(pNroConsulta);

                if (clsEEntelDetalleList.Count == 0)
                    throw new Exception("Error la consulta no tiene detalle.");

                clsEEntelDetalleList = clsEEntelDetalleList.Where(x => x.pNomMetodo == "PAGOVENTA")
                    .OrderByDescending(x => x.pPeriodo).ToList();
                var fechaReg = DateTime.Now;
                var listaDatosEntrada = new List<WcfEntel.datosEntrada>();
                foreach (var detalle in clsEEntelDetalleList)
                {
                    detalle.pHora = fechaReg.ToString(this.gHoraFormato);
                    var datosEntrada = mtdCrearDatosEntrada(detalle);
                    datosEntrada.transaccion = "R";
                    datosEntrada.cuenta = clsEEntel.pCuenta;
                    datosEntrada.observacion = "RE91";

                    listaDatosEntrada.Add(datosEntrada);
                }

                mtdPagoVenta(listaDatosEntrada, clsEEntelDetalleList, nombreMetodo, source);

                return clsEEntel;
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Este método nos permite Anular un pago
        /// </summary>
        /// <returns><see cref="Entidades.clsEEntel"/></returns>
        public Entidades.clsEEntel mtdAnular(int pNroConsulta)
        {
            try
            {
                var nombreMetodo = "ANULAR";
                var source = "clsNEntelWS.mtdAnularPostPago";

                clsNUtils.escribirLogInfo("pNroConsulta: " + pNroConsulta, source);

                var clsEEntel = mtdObtenerConsulta(pNroConsulta);
                var clsEEntelDetalleList = mtdObtenerDetalle(pNroConsulta);

                if (clsEEntelDetalleList.Count == 0)
                    throw new Exception("Error la consulta no tiene detalle.");

                clsEEntelDetalleList = clsEEntelDetalleList.Where(x => x.pNomMetodo == "PAGOVENTA")
                    .OrderByDescending(x => x.pPeriodo).ToList();

                var fechaReg = DateTime.Now;
                var listaDatosEntrada = new List<WcfEntel.datosEntrada>();
                foreach (var detalle in clsEEntelDetalleList)
                {
                    detalle.pHora = fechaReg.ToString(this.gHoraFormato);
                    var datosEntrada = mtdCrearDatosEntrada(detalle);
                    datosEntrada.transaccion = "A";
                    datosEntrada.cuenta = clsEEntel.pCuenta;
                    datosEntrada.observacion = "RE91";

                    listaDatosEntrada.Add(datosEntrada);
                }

                mtdPagoVenta(listaDatosEntrada, clsEEntelDetalleList, nombreMetodo, source);

                return clsEEntel;
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }

        public Entidades.clsEEntel mtdPagoPrePago(int pNroConsulta, decimal pMtoTotal, string pDepto,
            string pCiudad, string pEntidad, string pAgencia, string pOperador, string pTipoCobranza, string pNombreNit, string pNroNit, string pNroTransaccion,
            string pTelefonoRef, string pTipoDocumento, string pComplemento, string pEmail, string pDireccionSucursal)
        {
            try
            {
                var nombreMetodo = "PAGOVENTA";
                var source = "clsNEntelWS.mtdPagoPrePago";

                clsNUtils.escribirLogInfo("pNroConsulta: " + pNroConsulta + ", pMtoTotal: " + pMtoTotal, source);

                var clsEEntel = mtdObtenerConsulta(pNroConsulta);
                var clsEEntelDetalleList = mtdObtenerDetalle(pNroConsulta);

                if (clsEEntelDetalleList.Count == 0)
                    throw new Exception("Error la consulta no tiene detalle.");

                clsEEntelDetalleList = clsEEntelDetalleList.Where(x => x.pNomMetodo == "CONSULTAPREPAGO")
                    .OrderBy(x => x.pPeriodo).ToList();

                var listaDatosEntrada = new List<WcfEntel.datosEntrada>();
                var fechaReg = DateTime.Now;

                var clsEEntelDetalleListPagados = new List<Entidades.clsEEntelDetalle>();
                foreach (var detalle in clsEEntelDetalleList)
                {
                    detalle.pLoteDosificacion = "0";
                    detalle.pNumeroRenta = "0";
                    detalle.pMtoPagar = pMtoTotal;
                    detalle.pDepto = pDepto;
                    detalle.pCiudad = pCiudad;
                    detalle.pEntidad = pEntidad;
                    detalle.pAgencia = pAgencia;
                    detalle.pOperador = pOperador;
                    detalle.pTipoCobranza = pTipoCobranza;
                    detalle.pFecha = fechaReg;
                    detalle.pHora = fechaReg.ToString(this.gHoraFormato);
                    detalle.pCuenta = clsEEntel.pCuenta;
                    detalle.pRazonSocial = pNombreNit;
                    detalle.pNit = pNroNit;
                    detalle.pNombre = pNroTransaccion;
                    /// <summary>
                    /// Cambios Nuevos parametros ENTEL
                    /// </summary>
                    detalle.pTelefonoRef = pTelefonoRef;
                    detalle.pTipoDocumento = pTipoDocumento;
                    detalle.pComplemento = pComplemento;
                    detalle.pEmail = pEmail;
                    detalle.pDireccionSucursal = pDireccionSucursal;

                    clsEEntelDetalleListPagados.Add(detalle);

                    var datosEntrada = mtdCrearDatosEntrada(detalle);
                    datosEntrada.transaccion = "P";
                    datosEntrada.observacion = "";

                    listaDatosEntrada.Add(datosEntrada);
                }
                mtdPagoVenta(listaDatosEntrada, clsEEntelDetalleListPagados, nombreMetodo, source);

                return clsEEntel;
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Este método recupera los datos de una factura para ser impresa
        /// </summary>
        /// <param name="loteDosificacion">Dato retornado en la acreditación, Método Pago Venta o bien el valor del método Detalle cliente si es postpago</param>
        /// <param name="numeroFactura">Dato retornado en la acreditación, Método Pago Venta o bien el valor del método Detalle cliente si es postpago</param>
        /// <param name="tipoFactura">indicador del formato de la factura a imprimir, valores posibles: NOR</param>
        /// <returns><see cref="Entidades.clsEEntel"/></returns>
        public Entidades.clsEEntel mtdImpresion(string pLoteDosificacion, string pNumeroRenta, string pTipoFactura)
        {
            try
            {
                var nombreMetodo = "IMPRESION";
                var source = "clsNEntelWS.mtdImpresion";
                var sourceEntel = "WcfEntel.ServiciosPostEntelClient.Impresion";
                var datosEntrada = mtdCrearDatosEntrada();
                datosEntrada.loteDosificacion = pLoteDosificacion;
                datosEntrada.numeroRenta = pNumeroRenta;
                datosEntrada.tipoFactura = pTipoFactura;
                clsNUtils.escribirLogInfo(datosEntrada, sourceEntel + ".DatosEntrada");
                System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidCert);
                var respuesta = this.wcfEntel.Impresion(datosEntrada);
                clsNUtils.escribirLogInfo(respuesta, sourceEntel + ".Respuesta");

                var clsEEntel = mtdCrearClsEEntel(nombreMetodo, "", "",
                    "", respuesta.flagError, respuesta.mensaje);

                clsNUtils.escribirLogInfo(clsEEntel.ToString(), source);

                clsEEntel = mtdInsertarConsulta(clsEEntel);

                if (!string.IsNullOrWhiteSpace(respuesta.flagError) && respuesta.flagError == "S")
                    throw new EntelException(respuesta.flagError, respuesta.mensaje);
                int orden = 1; int orden2 = 1; int nroPag = 1;
                StringBuilder factura = new StringBuilder();
                if (respuesta.detalle != null)
                    foreach (var item in respuesta.detalle)
                    {

                        var clsEEntelImpresion = new Entidades.clsEEntelImpresion();
                        clsEEntelImpresion.pNroConsulta = clsEEntel.pNroConsulta;
                        clsEEntelImpresion.pOrden = orden;
                        clsEEntelImpresion.pOrden2 = orden2;
                        clsEEntelImpresion.pNroPag = nroPag;
                        clsEEntelImpresion.pLoteDosificacion = pLoteDosificacion;
                        clsEEntelImpresion.pNumeroRenta = pNumeroRenta;
                        clsEEntelImpresion.pTipoFactura = pTipoFactura;
                        clsEEntelImpresion.pDetalle = item.detalle;
                        if (orden2 == 60) /// 60 ->> cantidad de lineas por cada hoja de impresion
                        {
                            nroPag++;
                            orden2 = 0;
                        }
                        orden++; orden2++;
                        factura.AppendLine(item.detalle);
                        clsNUtils.escribirLogInfo(clsEEntelImpresion.ToString(), source);
                        mtdInsertarImpresion(clsEEntelImpresion);
                    }
                clsNUtils.escribirLogInfo(factura.ToString(), source);
                return clsEEntel;
            }
            catch (EndpointNotFoundException ex)
            {
                clsNUtils.escribirLogError(ex, gErrorServicioEntel);
                throw new Exception(gErrorServicioEntel);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Este método recupera los datos de las transacciones realizadas por una entidad financiera en un rango de fechas específico.
        /// </summary>
        /// <param name="fechaDesde">fecha desde transacciones Formato militar: YYYYMMDD</param>
        /// <param name="fechaHasta">fecha hasta transacciones Formato militar: YYYYMMDD</param>
        /// <param name="estado">códigos asignado por ENTEL a la entidad Ejemplo: P(Pagado), R(Revertido), A(Anulado)</param>
        /// <returns><see cref="Entidades.clsEEntel"/></returns>
        public Entidades.clsEEntel mtdObtenerReporte(DateTime fechaDesde, DateTime fechaHasta, string estado)
        {
            try
            {
                var nombreMetodo = "OBTENERREPORTE";
                var source = "clsNEntelWS.mtdObtenerReporte";
                var sourceEntel = "WcfEntel.ServiciosPostEntelClient.obtenerReporte";
                var datosEntrada = mtdCrearDatosEntrada();
                datosEntrada.fechaDesde = fechaDesde.ToString(gFechaFormato);
                datosEntrada.fechaHasta = fechaHasta.ToString(gFechaFormato);
                datosEntrada.estado = estado;
                clsNUtils.escribirLogInfo(datosEntrada, sourceEntel + ".DatosEntrada");
                System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidCert);
                var respuesta = this.wcfEntel.obtenerReporte(datosEntrada);
                clsNUtils.escribirLogInfo(respuesta, sourceEntel + ".Respuesta");

                var clsEEntel = mtdCrearClsEEntel(nombreMetodo, "", "",
                    "", respuesta.flagError, respuesta.mensaje);

                clsNUtils.escribirLogInfo(clsEEntel.ToString(), source);

                clsEEntel = mtdInsertarConsulta(clsEEntel);

                if (!string.IsNullOrWhiteSpace(respuesta.flagError) && respuesta.flagError == "S")
                    throw new EntelException(respuesta.flagError, respuesta.mensaje);

                if (respuesta.detalle != null)
                    foreach (var item in respuesta.detalle)
                    {
                        var clsEEntelReporte = new Entidades.clsEEntelReporte();
                        clsEEntelReporte.pNroConsulta = clsEEntel.pNroConsulta;
                        clsEEntelReporte.pAgencia = item.agencia;
                        clsEEntelReporte.pCiudad = item.ciudad;
                        clsEEntelReporte.pCuenta = item.cuenta;
                        clsEEntelReporte.pDepartamento = item.departamento;
                        clsEEntelReporte.pDosificacionPrepago = item.dosificacionPrepago;
                        clsEEntelReporte.pEstado = item.estado;
                        clsEEntelReporte.pFecha = DateTime.ParseExact(item.fecha, gFechaFormato, CultureInfo.InvariantCulture);
                        clsEEntelReporte.pHora = item.hora;
                        clsEEntelReporte.pImporte = Convert.ToDecimal(item.importe, gCultureInfo);
                        clsEEntelReporte.pLote = item.lote;
                        clsEEntelReporte.pNit = item.nit;
                        clsEEntelReporte.pNumeroRenta = item.numeroRenta;
                        clsEEntelReporte.pNumeroRentaPrepago = item.numeroRentaPrepago;
                        clsEEntelReporte.pOperador = item.operador;
                        clsEEntelReporte.pPeriodo = item.periodo;
                        clsEEntelReporte.pRazonSocial = item.razonSocial;
                        clsEEntelReporte.pTipoPago = item.tipoPago;

                        clsNUtils.escribirLogInfo(clsEEntelReporte.ToString(), source);
                        mtdInsertarReporte(clsEEntelReporte);
                    }

                return clsEEntel;
            }
            catch (EndpointNotFoundException ex)
            {
                clsNUtils.escribirLogError(ex, gErrorServicioEntel);
                throw new Exception(gErrorServicioEntel);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }




        private WcfEntel.datosEntrada mtdCrearDatosEntrada(Entidades.clsEEntelDetalle pClsEEntelDetalle = null)
        {
            var datos = new WcfEntel.datosEntrada();
            datos.usuario = this.gUsuario;
            datos.password = this.gPassword;
            datos.entidadFinanciera = this.gEntidadFinanciera;
            datos.entidadCobranza = this.gEntidadCobranza;
            datos.entidad = this.gEntidadFinanciera;
            datos.formaPago = this.gFormaPago;
            datos.tipoFactura = this.gTipoFactura;
            if (pClsEEntelDetalle != null)
            {
                datos.loteDosificacion = pClsEEntelDetalle.pLoteDosificacion;
                datos.numeroRenta = pClsEEntelDetalle.pNumeroRenta;
                datos.montoTotal = pClsEEntelDetalle.pMtoPagar.ToString(gCultureInfo);
                datos.departamento = pClsEEntelDetalle.pDepto;
                datos.ciudad = pClsEEntelDetalle.pCiudad;
                // datos.entidad = pClsEEntelDetalle.pEntidad; --> siempre va ser el mismo valor
                datos.agencia = pClsEEntelDetalle.pAgencia;
                datos.operador = pClsEEntelDetalle.pOperador;
                datos.fecha = pClsEEntelDetalle.pFecha.ToString(this.gFechaFormato);
                datos.hora = pClsEEntelDetalle.pHora;
                datos.tipoCobranza = pClsEEntelDetalle.pTipoCobranza;
                datos.cuenta = pClsEEntelDetalle.pCuenta;
                datos.nombreFactura = pClsEEntelDetalle.pRazonSocial;
                datos.nitFactura = pClsEEntelDetalle.pNit;
                /// <summary>
                /// Cambios Nuevos parametros ENTEL
                /// </summary>
                /// 
                datos.telefonoRef = pClsEEntelDetalle.pTelefonoRef;
                datos.tipoDocumento = pClsEEntelDetalle.pTipoDocumento;
                datos.complemento = pClsEEntelDetalle.pComplemento;
                datos.email = pClsEEntelDetalle.pEmail;
                datos.direccionSucursal = pClsEEntelDetalle.pDireccionSucursal;
            }
            return datos;
        }

        private Entidades.clsEEntel mtdCrearClsEEntel(string pNomMetodo = "", string pCodBusqueda = "",
            string pCodAcceso = "", string pCuenta = "", string pCodError = "", string pMsjError = "",
            DateTime? pFecha = null, DateTime? pHora = null, string pTelefonoRef = "", string pTipoDocumento = "", string pComplemento = "", string pEmail = "", string pRazonSocial="", string pNit ="")
        {
            var clsEEntel = new Entidades.clsEEntel();
            clsEEntel.pNomMetodo = pNomMetodo;
            clsEEntel.pCodBusqueda = pCodBusqueda;
            clsEEntel.pCodAcceso = pCodAcceso;
            clsEEntel.pEntCobranza = this.gEntidadCobranza;
            clsEEntel.pCuenta = pCuenta;
            clsEEntel.pFecha = pFecha ?? DateTime.Now;
            clsEEntel.pHora = pHora == null ? DateTime.Now.ToShortTimeString() : pHora.Value.ToShortTimeString();
            clsEEntel.pCodError = pCodError;
            clsEEntel.pMsjError = pMsjError;

            clsEEntel.pTelefonoRef = pTelefonoRef;
            clsEEntel.pTipoDocumento = pTipoDocumento;
            clsEEntel.pComplemento = pComplemento;
            clsEEntel.pEmail = pEmail;
            clsEEntel.pRazonSocial = pRazonSocial;
            clsEEntel.pNit = pNit;
            return clsEEntel;
        }

        private Entidades.clsEEntelDetalle mtdCrearClsEEntelDet(int pNroConsulta, string pNombreMetodo)
        {
            var clsDetalle = new Entidades.clsEEntelDetalle();
            clsDetalle.pNroConsulta = pNroConsulta;
            clsDetalle.pNomMetodo = pNombreMetodo;
            clsDetalle.pFecha = DateTime.Now;
            clsDetalle.pHora = DateTime.Now.ToString(this.gHoraFormato);
            clsDetalle.pEntidad = this.gEntidadFinanciera;
            return clsDetalle;
        }





        private Entidades.clsEEntel mtdInsertarConsulta(Entidades.clsEEntel clsEEntel)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                var strSql = "SPR_RCT_SERVICIO_ENTEL_MAE";
                var listaParametros = new List<OracleParameter>();
                listaParametros.Add(new OracleParameter("pNomMetodo", OracleDbType.Varchar2, clsEEntel.pNomMetodo, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pCodBusqueda", OracleDbType.Varchar2, clsEEntel.pCodBusqueda, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pCodAcceso", OracleDbType.Varchar2, clsEEntel.pCodAcceso, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pEntCobranza", OracleDbType.Varchar2, clsEEntel.pEntCobranza, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pCuenta", OracleDbType.Varchar2, clsEEntel.pCuenta, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pFecha", OracleDbType.Date, clsEEntel.pFecha, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pHora", OracleDbType.Varchar2, clsEEntel.pHora, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pCodError", OracleDbType.Varchar2, clsEEntel.pCodError, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pMsjError", OracleDbType.Varchar2, clsEEntel.pMsjError, ParameterDirection.Input));

                listaParametros.Add(new OracleParameter("pComplemento", OracleDbType.Varchar2, clsEEntel.pComplemento, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pEmail", OracleDbType.Varchar2, clsEEntel.pEmail, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pTelefonoRef", OracleDbType.Varchar2, clsEEntel.pTelefonoRef, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pTipoDocumento", OracleDbType.Varchar2, clsEEntel.pTipoDocumento, ParameterDirection.Input));
                
                listaParametros.Add(new OracleParameter("pNit", OracleDbType.Varchar2, clsEEntel.pNit, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pRazonSocial", OracleDbType.Varchar2, clsEEntel.pRazonSocial, ParameterDirection.Input));

                listaParametros.Add(new OracleParameter("pDataSet", OracleDbType.RefCursor, ParameterDirection.Output));
                var dataSet = db.mtdEjecutarSPconRetorno(listaParametros, strSql);
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    throw new Exception("Error al insertar, no se devolvio ningun resultado.");

                return mtdMapToEntidad(dataSet.Tables[0].Rows[0]);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }

        private void mtdInsertarDetalle(Entidades.clsEEntelDetalle clsDetalle)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                var strSql = "SPR_RCT_SERVICIO_ENTEL_DET";
                var listaParametros = new List<OracleParameter>();
                listaParametros.Add(new OracleParameter("pNroConsulta", OracleDbType.Int32, clsDetalle.pNroConsulta, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pNomMetodo", OracleDbType.Varchar2, clsDetalle.pNomMetodo, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pLista", OracleDbType.Varchar2, clsDetalle.pLista, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pTipoCuenta", OracleDbType.Varchar2, clsDetalle.pTipoCuenta, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pNombre", OracleDbType.Varchar2, clsDetalle.pNombre, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pCuenta", OracleDbType.Varchar2, clsDetalle.pCuenta, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pMtoDeuda", OracleDbType.Decimal, clsDetalle.pMtoDeuda, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pServicio", OracleDbType.Varchar2, clsDetalle.pServicio, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pFormaPago", OracleDbType.Varchar2, clsDetalle.pFormaPago, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pDesFormaPago", OracleDbType.Varchar2, clsDetalle.pDesFormaPago, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pDesServicio", OracleDbType.Varchar2, clsDetalle.pDesServicio, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pPeriodo", OracleDbType.Varchar2, clsDetalle.pPeriodo, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pLoteDosificacion", OracleDbType.Varchar2, clsDetalle.pLoteDosificacion, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pNumeroRenta", OracleDbType.Varchar2, clsDetalle.pNumeroRenta, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pMtoPagar", OracleDbType.Decimal, clsDetalle.pMtoPagar, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pTipoFactura", OracleDbType.Varchar2, clsDetalle.pTipoFactura, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pRazonSocial", OracleDbType.Varchar2, clsDetalle.pRazonSocial, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pNit", OracleDbType.Varchar2, clsDetalle.pNit, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pFactura", OracleDbType.Varchar2, clsDetalle.pFactura, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pSaldo", OracleDbType.Decimal, clsDetalle.pSaldo, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pMtoMinimo", OracleDbType.Decimal, clsDetalle.pMtoMinimo, ParameterDirection.Input));

                listaParametros.Add(new OracleParameter("pDepto", OracleDbType.Varchar2, clsDetalle.pDepto, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pAgencia", OracleDbType.Varchar2, clsDetalle.pAgencia, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pCiudad", OracleDbType.Varchar2, clsDetalle.pCiudad, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pEntidad", OracleDbType.Varchar2, clsDetalle.pEntidad, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pOperador", OracleDbType.Varchar2, clsDetalle.pOperador, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pTipoCobranza", OracleDbType.Varchar2, clsDetalle.pTipoCobranza, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pFecha", OracleDbType.Date, clsDetalle.pFecha, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pHora", OracleDbType.Varchar2, clsDetalle.pHora, ParameterDirection.Input));
                db.mtdEjecutarSP(listaParametros, strSql);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }

        private void mtdInsertarReporte(Entidades.clsEEntelReporte clsReporte)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                var strSql = "SPR_RCT_SERVICIO_ENTEL_RPT";
                var listaParametros = new List<OracleParameter>();
                listaParametros.Add(new OracleParameter("pNroConsulta", OracleDbType.Int32, clsReporte.pNroConsulta, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pAgencia", OracleDbType.Varchar2, clsReporte.pAgencia, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pCiudad", OracleDbType.Varchar2, clsReporte.pCiudad, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pCuenta", OracleDbType.Varchar2, clsReporte.pCuenta, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pDepartamento", OracleDbType.Varchar2, clsReporte.pDepartamento, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pDosificacionPrepago", OracleDbType.Varchar2, clsReporte.pDosificacionPrepago, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pEstado", OracleDbType.Varchar2, clsReporte.pEstado, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pFecha", OracleDbType.Date, clsReporte.pFecha, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pHora", OracleDbType.Varchar2, clsReporte.pHora, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pImporte", OracleDbType.Decimal, clsReporte.pImporte, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pLote", OracleDbType.Varchar2, clsReporte.pLote, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pNit", OracleDbType.Varchar2, clsReporte.pNit, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pNumeroRenta", OracleDbType.Varchar2, clsReporte.pNumeroRenta, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pNumeroRentaPrepago", OracleDbType.Varchar2, clsReporte.pNumeroRentaPrepago, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pOperador", OracleDbType.Varchar2, clsReporte.pOperador, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pPeriodo", OracleDbType.Varchar2, clsReporte.pPeriodo, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pRazonSocial", OracleDbType.Varchar2, clsReporte.pRazonSocial, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pTipoPago", OracleDbType.Varchar2, clsReporte.pTipoPago, ParameterDirection.Input));

                db.mtdEjecutarSP(listaParametros, strSql);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }

        private void mtdInsertarImpresion(Entidades.clsEEntelImpresion clsImpresion)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                var strSql = "INSERT INTO GANADERO.RCT_SERVICIO_ENTEL_IMP";
                strSql += " (NRO_CONSULTA, ORDEN, ORDEN2, NRO_PAG, LOTEDOSIFICACION, NUMERORENTA, TIPOFACTURA, DETALLE, TZ_LOCK)";
                strSql += " VALUES(:pNroConsulta, :pOrden, :pOrden2, :pNroPag, :pLoteDosificacion, :pNumeroRenta, :pTipoFactura, :pDetalle, 0)";
                var listaParametros = new List<OracleParameter>();
                listaParametros.Add(new OracleParameter("pNroConsulta", OracleDbType.Int32, clsImpresion.pNroConsulta, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pOrden", OracleDbType.Varchar2, clsImpresion.pOrden, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pOrden2", OracleDbType.Varchar2, clsImpresion.pOrden2, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pNroPag", OracleDbType.Varchar2, clsImpresion.pNroPag, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pLoteDosificacion", OracleDbType.Varchar2, clsImpresion.pLoteDosificacion, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pNumeroRenta", OracleDbType.Varchar2, clsImpresion.pNumeroRenta, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pTipoFactura", OracleDbType.Varchar2, clsImpresion.pTipoFactura, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pDetalle", OracleDbType.Varchar2, clsImpresion.pDetalle, ParameterDirection.Input));

                if (db.mtdEjecutarComandoTxt(listaParametros, strSql) == -1)
                    clsNUtils.escribirLogError(clsImpresion.ToString(), "clsNEntelWS.mtdInsertarReporte");
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }

        public Entidades.clsEEntel mtdObtenerConsulta(int nroConsulta)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                var sql = string.Format("Select * from RCT_SERVICIO_ENTEL_MAE where NRO_CONSULTA = {0}", nroConsulta);
                var dataSet = db.mtdEjecutarConsulta(sql);
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    throw new Exception("Error no se encontro ningun resultado.");
                return mtdMapToEntidad(dataSet.Tables[0].Rows[0]);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }

        public List<Entidades.clsEEntelDetalle> mtdObtenerDetalle(int nroConsulta)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                var sql = string.Format("Select * from RCT_SERVICIO_ENTEL_DET where NRO_CONSULTA = {0}", nroConsulta);
                var dataSet = db.mtdEjecutarConsulta(sql);
                var lista = new List<Entidades.clsEEntelDetalle>();
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    return lista;

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var entidad = mtdMapToEntidadDetalle(row);
                    lista.Add(entidad);
                }
                return lista;
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw ex;
            }
        }

        public DataTable mtdObtenerDeudasPorPeriodo(int pNroConsulta)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                var sqlDeudas = "SELECT * FROM VW_RCT_DEUDAENTEL WHERE NRO_CONSULTA=" + pNroConsulta;
                var dsDeudasPorPeriodo = db.mtdEjecutarConsulta(sqlDeudas);
                if (dsDeudasPorPeriodo == null || dsDeudasPorPeriodo.Tables.Count == 0 ||
                    dsDeudasPorPeriodo.Tables[0].Rows.Count == 0)
                    throw new Exception("No existen deudas.");
                return dsDeudasPorPeriodo.Tables[0];
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex, "NroConsulta: " + pNroConsulta);
                throw;
            }

        }






        private Entidades.clsEEntel mtdMapToEntidad(DataRow dataRow)
        {
            if (dataRow == null) return null;
            var clsEEntel = new Entidades.clsEEntel();
            clsEEntel.pNroConsulta = Convert.ToInt32(dataRow["NRO_CONSULTA"]);
            clsEEntel.pNomMetodo = Convert.ToString(dataRow["NOM_METODO"]);
            clsEEntel.pCodBusqueda = Convert.ToString(dataRow["COD_BUSQUEDA"]);
            clsEEntel.pCodAcceso = Convert.ToString(dataRow["COD_ACCESO"]);
            clsEEntel.pEntCobranza = Convert.ToString(dataRow["ENT_COBRANZA"]);
            clsEEntel.pCuenta = Convert.ToString(dataRow["CUENTA"]);
            clsEEntel.pFecha = Convert.ToDateTime(dataRow["FECHA"]);
            clsEEntel.pHora = Convert.ToString(dataRow["HORA"]);
            clsEEntel.pCodError = Convert.ToString(dataRow["COD_ERROR"]);
            clsEEntel.pMsjError = Convert.ToString(dataRow["MENSAJE_ERROR"]);
            /////
            ///// Cambios ENTEL 
            /////
            clsEEntel.pTelefonoRef = Convert.ToString(dataRow["TELEFONOREF"]);
            clsEEntel.pTipoDocumento = Convert.ToString(dataRow["TIPODOCUMENTO"]);
            clsEEntel.pComplemento = Convert.ToString(dataRow["COMPLEMENTO"]);
            clsEEntel.pEmail = Convert.ToString(dataRow["EMAIL"]);
            return clsEEntel;
        }

        private Entidades.clsEEntelDetalle mtdMapToEntidadDetalle(DataRow dataRow)
        {
            if (dataRow == null) return null;
            var clsDetalle = new Entidades.clsEEntelDetalle();
            clsDetalle.pNroConsulta = Convert.ToInt32(dataRow["NRO_CONSULTA"]);
            clsDetalle.pNomMetodo = Convert.ToString(dataRow["NOM_METODO"]);
            clsDetalle.pLista = Convert.ToString(dataRow["LISTA"]);
            clsDetalle.pTipoCuenta = Convert.ToString(dataRow["TIPO_CUENTA"]);
            clsDetalle.pNombre = Convert.ToString(dataRow["NOMBRE"]);
            clsDetalle.pCuenta = Convert.ToString(dataRow["CUENTA"]);
            clsDetalle.pMtoDeuda = Convert.ToDecimal(dataRow["MTO_DEUDA"], gCultureInfo);
            clsDetalle.pServicio = Convert.ToString(dataRow["SERVICIO"]);
            clsDetalle.pFormaPago = Convert.ToString(dataRow["FORMA_PAGO"]);
            clsDetalle.pDesFormaPago = Convert.ToString(dataRow["DES_FORMAPAGO"]);
            clsDetalle.pDesServicio = Convert.ToString(dataRow["DES_SERVICIO"]);
            clsDetalle.pPeriodo = Convert.ToString(dataRow["PERIODO"]);
            clsDetalle.pLoteDosificacion = Convert.ToString(dataRow["LOTEDOSIFICACION"]);
            clsDetalle.pNumeroRenta = Convert.ToString(dataRow["NUMERORENTA"]);
            clsDetalle.pMtoPagar = Convert.ToDecimal(dataRow["MTO_PAGAR"], gCultureInfo);
            clsDetalle.pTipoFactura = Convert.ToString(dataRow["TIPO_FACTURA"]);
            clsDetalle.pRazonSocial = Convert.ToString(dataRow["RAZON_SOCIAL"]);
            clsDetalle.pNit = Convert.ToString(dataRow["NIT"]);
            clsDetalle.pFactura = Convert.ToString(dataRow["FACTURA"]);
            clsDetalle.pSaldo = Convert.ToDecimal(dataRow["SALDO"], gCultureInfo);
            clsDetalle.pMtoMinimo = Convert.ToDecimal(dataRow["MTO_MINIMO"], gCultureInfo);
            clsDetalle.pDepto = Convert.ToString(dataRow["DEPARTAMENTO"]);
            clsDetalle.pCiudad = Convert.ToString(dataRow["CIUDAD"]);
            clsDetalle.pEntidad = Convert.ToString(dataRow["ENTIDAD"]);
            clsDetalle.pAgencia = Convert.ToString(dataRow["AGENCIA"]);
            clsDetalle.pOperador = Convert.ToString(dataRow["OPERADOR"]);
            clsDetalle.pTipoCobranza = Convert.ToString(dataRow["TIPOCOBRANZA"]);
            clsDetalle.pFecha = Convert.IsDBNull(dataRow["FECHA"]) ? DateTime.Now : Convert.ToDateTime(dataRow["FECHA"]);
            clsDetalle.pHora = Convert.ToString(dataRow["HORA"]);

            return clsDetalle;
        }

        private bool ValidCert(Object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }


    }

    public class EntelException : Exception
    {
        public string pMsjError { get; set; }
        public string pCodError { get; set; }
        public EntelException(string cod, string msj)
            : base(msj)
        {
            this.pCodError = cod;
            this.pMsjError = msj;
        }
    }
}
