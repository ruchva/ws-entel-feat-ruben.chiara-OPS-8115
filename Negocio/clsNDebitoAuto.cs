using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using BG.Topaz.Enlace;

namespace Negocio
{
    public class clsNDebitoAuto
    {
        /// <summary>
        /// Consulta las deudas del convenio Entel e inserta el detalle
        /// </summary>
        /// <param name="pCodConvenio"></param>
        /// <param name="pCodPlaza"></param>
        /// <param name="pUsuario"></param>
        /// <returns></returns>
        public Entidades.clsEDebitoAuto mtdDebitoEntel(int pCodConvenio, int pCodPlaza, string pUsuario)
        {
            try
            {
                var source = "clsNDebitoAuto.mtdDebitoEntel";
                clsNUtils.escribirLogInfo("Consultando: pCodConvenio: " + pCodConvenio + ", pCodPlaza: " + pCodPlaza, source);
                var db = new Datos.clsOracleConexion();
                var sql = @"Select *
                        from ganadero.RCT_SUSCRIPCION_SERVICIO      
                        where id_convenio = {0}
                            and estado = 1  
                            and bloqueada = 'N'   
                            and tz_lock = 0    
                            and sucursal_origen in 
                                (select sucursal 
                                    from ganadero.sucursales 
                                    where (zonaComercial = {1} OR {1} = 0)) ";  /*and rownum <= 100*/
                sql = string.Format(sql, pCodConvenio, pCodPlaza);

                var dataSet = db.mtdEjecutarConsulta(sql);
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    throw new Exception("Error no se encontro ningun resultado.");

                var clsDebito = new Entidades.clsEDebitoAuto();
                clsDebito.pCodConvenio = pCodConvenio;
                clsDebito.pCodPlaza = pCodPlaza;
                clsDebito.pFecha = DateTime.Now;
                clsDebito.pUsuario = pUsuario;

                clsNUtils.escribirLogInfo(clsDebito, source);
                clsDebito = mtdInsertar(clsDebito);

                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                {
                    var clsDebitoDet = new Entidades.clsEDebitoAutoDet();
                    clsDebitoDet.pCodigoMae = clsDebito.pCodigo;
                    clsDebitoDet.pServicioRef = Convert.ToString(dataRow["SERVICIO_REFERENCIA"]);
                    clsDebitoDet.pJtsOidSaldo = Convert.ToInt32(dataRow["JTS_OID_SALDO"]);
                    clsDebitoDet.pNroConsulta = 0;
                    clsDebitoDet.pEstado = Entidades.enumEEDebitoAutoDetEstado.Pendiente;
                    clsDebitoDet.pDescError = "";
                    try
                    {
                        var clsNEntel = new clsNEntelWS();
                        var clsEEntel = clsNEntel.mtdDetalleCliente(clsDebitoDet.pServicioRef);
                        clsDebitoDet.pNroConsulta = clsEEntel.pNroConsulta;
                    }
                    catch (Exception ex)
                    {
                        clsDebitoDet.pEstado = Entidades.enumEEDebitoAutoDetEstado.Error; // E = Error
                        clsDebitoDet.pDescError = ex.Message;
                    }
                    clsNUtils.escribirLogInfo(clsDebitoDet, source);
                    mtdInsertarDetalle(clsDebitoDet);

                    clsDebito.pListDetalle.Add(clsDebitoDet);
                }

                return clsDebito;
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw;
            }
        }
        public Entidades.clsEDebitoAuto mtdDebitoEntelXUno(int pCodigo , int pCodConvenio, int pCodPlaza, string pUsuario,string pServicioReferencia, int pJtsOid)
        {
            try
            {
                var source = "clsNDebitoAuto.mtdDebitoEntelXUno";
                clsNUtils.escribirLogInfoAuto("Consultando: pCodigo " + pCodigo  + " pCodConvenio: " + pCodConvenio + ", pCodPlaza: " + pCodPlaza, source + ", pServicioReferencia: " + pServicioReferencia);
               /* var db = new Datos.clsOracleConexion();
                var sql = @"Select *
                        from ganadero.RCT_SUSCRIPCION_SERVICIO      
                        where id_convenio = {0}
                            and estado = 1  
                            and bloqueada = 'N'   
                            and tz_lock = 0     
                            and sucursal_origen in 
                                (select sucursal 
                                    from ganadero.sucursales 
                                    where (zonaComercial = {1} OR {1} = 0)) ";  
                sql = string.Format(sql, pCodConvenio, pCodPlaza);

                var dataSet = db.mtdEjecutarConsulta(sql);
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    throw new Exception("Error no se encontro ningun resultado.");
                */

                var clsDebito = new Entidades.clsEDebitoAuto();
                clsDebito.pCodConvenio = pCodConvenio;
                clsDebito.pCodPlaza = pCodPlaza;
                clsDebito.pFecha = DateTime.Now;
                clsDebito.pUsuario = pUsuario;
                clsDebito.pCodigo = pCodigo;

                clsNUtils.escribirLogInfoAuto(clsDebito, source);
                
                /*clsDebito = mtdInsertar(clsDebito);*/

                //foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                //{
                    var clsDebitoDet = new Entidades.clsEDebitoAutoDet();
                    clsDebitoDet.pCodigoMae = clsDebito.pCodigo;
                    clsDebitoDet.pServicioRef = Convert.ToString(pServicioReferencia);
                    clsDebitoDet.pJtsOidSaldo = Convert.ToInt32(pJtsOid);
                    clsDebitoDet.pNroConsulta = 0;
                    clsDebitoDet.pEstado = Entidades.enumEEDebitoAutoDetEstado.Pendiente;
                    clsDebitoDet.pDescError = "";
                    try
                    {
                        var clsNEntel = new clsNEntelWS();
                        var clsEEntel = clsNEntel.mtdDetalleCliente(clsDebitoDet.pServicioRef);
                        clsDebitoDet.pNroConsulta = clsEEntel.pNroConsulta;
                    }
                    catch (Exception ex)
                    {
                        clsDebitoDet.pEstado = Entidades.enumEEDebitoAutoDetEstado.Error; // E = Error
                        clsDebitoDet.pDescError = ex.Message;
                    }
                    clsNUtils.escribirLogInfoAuto(clsDebitoDet, source);
                    mtdInsertarDetalle(clsDebitoDet);

                    clsDebito.pListDetalle.Add(clsDebitoDet);
                //}

                return clsDebito;
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogErrorAuto(ex);
                throw;
            }
        }
        public Entidades.clsEDebitoAuto mtdAplicarDebitoEntel(int pCodigoMae, string pUsuario, string pSucursal)
        {
            try
            {
                var source = "clsNDebitoAuto.mtdAplicarDebitoEntel";
                clsNUtils.escribirLogInfo("pCodigoMae: " + pCodigoMae + ", pUsuario: " + pUsuario + ", pSucursal: " + pSucursal, source);

                var entidad = mtdObtener2(pCodigoMae);
                var listaDetalle = mtdObtenerDetalle(pCodigoMae);
                foreach (var detalle in listaDetalle)
                {
                    try
                  {
                    clsNUtils.escribirLogInfo(detalle, source);

                    if (detalle.pEstado != Entidades.enumEEDebitoAutoDetEstado.Pendiente)
                        continue;// ya esta pagada o con error

                    var clsNEntel = new clsNEntelWS();
                    var clsEEntel = clsNEntel.mtdBuscarCliente("2", detalle.pServicioRef);
                    var detalleCuentaEntel = clsEEntel.pListDetalle.Where(
                        x => x.pLista == Entidades.enumEEntelDetalleTipo.Detalle).FirstOrDefault();
                    var detalleFormaPago = clsEEntel.pListDetalle.Where(x =>
                        x.pLista == Entidades.enumEEntelDetalleTipo.FormaPago).FirstOrDefault();


                    detalle.pEstado = Entidades.enumEEDebitoAutoDetEstado.Pagado;
                    detalle.pDescError = "";
                    try
                    {
                        if (detalleCuentaEntel == null)
                            throw new Exception("No se encontro el detalle de cuenta");
                        if (detalleFormaPago == null)
                            throw new Exception("No se encontro la forma de pago");

                        var dtDeudasPorPeriodo = clsNEntel.mtdObtenerDeudasPorPeriodo(detalle.pNroConsulta);

                        for (var i = 0; i < dtDeudasPorPeriodo.Rows.Count; i++)
                        {
                            var listaParametros = new List<clsParametrosProxy>();
                            listaParametros.Add(new clsParametrosProxy("JTS_OID_CUENTA", detalle.pJtsOidSaldo.ToString()));
                            listaParametros.Add(new clsParametrosProxy("NROCONSULTA", detalle.pNroConsulta.ToString()));
                            listaParametros.Add(new clsParametrosProxy("FORMAPAGO", detalleFormaPago.pFormaPago));
                            listaParametros.Add(new clsParametrosProxy("SERVICIOREF", detalleCuentaEntel.pCuenta));
                            clsNUtils.escribirLogInfo(listaParametros, source);

                            var clsEstadoTZ = new clsTopaz().EjecutarServicio("RCT_DebitoAuto_Entel",
                                clsEnumProxy.enumTipoADevolver.enObtenerDataTable, listaParametros);
                            clsNUtils.escribirLogInfo(clsEstadoTZ, source);
                            if (clsEstadoTZ.CodMsjeProceso == clsEnumProxy.enumEstadoEjecucion.enError)
                                throw new Exception(clsEstadoTZ.Mensaje);

                            var dtResultado = clsEstadoTZ.Resultado as DataTable;

                            if (dtResultado == null || dtResultado.Rows.Count == 0)
                                throw new Exception("Error la operacion Topaz no devuelto ningun resultado.");

                            if (dtResultado.Rows[0]["CODERROR"].ToString() == "1")
                                throw new Exception(dtResultado.Rows[0]["DESERROR"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        clsNUtils.escribirLogError(ex);
                        detalle.pEstado = Entidades.enumEEDebitoAutoDetEstado.Error;
                        detalle.pDescError = ex.Message;
                    }
                    mtdInsertarDetalle(detalle, true);
                    entidad.pListDetalle.Add(detalle);
                    clsNUtils.escribirLogInfo(detalle, source);
                 }
                catch (Exception ex)
                {
                        clsNUtils.escribirLogError(ex, "Error En For Principal " + detalle.pServicioRef);
                                            }
                }
                return entidad;
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw;
            }
        }

        public Entidades.clsEDebitoAuto mtdAplicarDebitoEntelxUno(int pCodigoMae, string pUsuario, string pSucursal,int  pNroConsulta)
        {
            try
            {
                var source = "clsNDebitoAuto.mtdAplicarDebitoEntel";
                clsNUtils.escribirLogInfoAuto("pCodigoMae: " + pCodigoMae + ", pUsuario: " + pUsuario + ", pSucursal: " + pSucursal + ", pNroConsulta: " + pNroConsulta, source);

                var entidad = mtdObtener2(pCodigoMae);
                var listaDetalle = mtdObtenerDetalleXUno(pCodigoMae, pNroConsulta);
                foreach (var detalle in listaDetalle)
                {
                    try
                    {
                        clsNUtils.escribirLogInfoAuto(detalle, source);

                        if (detalle.pEstado != Entidades.enumEEDebitoAutoDetEstado.Pendiente)
                            continue;// ya esta pagada o con error

                        var clsNEntel = new clsNEntelWS();
                        var clsEEntel = clsNEntel.mtdBuscarCliente("2", detalle.pServicioRef);
                        var detalleCuentaEntel = clsEEntel.pListDetalle.Where(
                            x => x.pLista == Entidades.enumEEntelDetalleTipo.Detalle).FirstOrDefault();
                        var detalleFormaPago = clsEEntel.pListDetalle.Where(x =>
                            x.pLista == Entidades.enumEEntelDetalleTipo.FormaPago).FirstOrDefault();


                        detalle.pEstado = Entidades.enumEEDebitoAutoDetEstado.Pagado;
                        detalle.pDescError = "";
                        try
                        {
                            if (detalleCuentaEntel == null)
                                throw new Exception("No se encontro el detalle de cuenta");
                            if (detalleFormaPago == null)
                                throw new Exception("No se encontro la forma de pago");

                            var dtDeudasPorPeriodo = clsNEntel.mtdObtenerDeudasPorPeriodo(detalle.pNroConsulta);

                            for (var i = 0; i < dtDeudasPorPeriodo.Rows.Count; i++)
                            {
                                var listaParametros = new List<clsParametrosProxy>();
                                listaParametros.Add(new clsParametrosProxy("JTS_OID_CUENTA", detalle.pJtsOidSaldo.ToString()));
                                listaParametros.Add(new clsParametrosProxy("NROCONSULTA", detalle.pNroConsulta.ToString()));
                                listaParametros.Add(new clsParametrosProxy("FORMAPAGO", detalleFormaPago.pFormaPago));
                                listaParametros.Add(new clsParametrosProxy("SERVICIOREF", detalleCuentaEntel.pCuenta));
                                clsNUtils.escribirLogInfoAuto(listaParametros, source);

                                var clsEstadoTZ = new clsTopaz().EjecutarServicio("RCT_DebitoAuto_Entel",
                                    clsEnumProxy.enumTipoADevolver.enObtenerDataTable, listaParametros);
                                clsNUtils.escribirLogInfoAuto(clsEstadoTZ, source);
                                if (clsEstadoTZ.CodMsjeProceso == clsEnumProxy.enumEstadoEjecucion.enError)
                                    throw new Exception(clsEstadoTZ.Mensaje);

                                var dtResultado = clsEstadoTZ.Resultado as DataTable;

                                if (dtResultado == null || dtResultado.Rows.Count == 0)
                                    throw new Exception("Error la operacion Topaz no devuelto ningun resultado.");

                                if (dtResultado.Rows[0]["CODERROR"].ToString() == "1")
                                    throw new Exception(dtResultado.Rows[0]["DESERROR"].ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            clsNUtils.escribirLogErrorAuto(ex);
                            detalle.pEstado = Entidades.enumEEDebitoAutoDetEstado.Error;
                            detalle.pDescError = ex.Message;
                        }
                        mtdInsertarDetalle(detalle, true);
                        entidad.pListDetalle.Add(detalle);
                        clsNUtils.escribirLogInfoAuto(detalle, source);
                    }
                    catch (Exception ex)
                    {
                        clsNUtils.escribirLogErrorAuto(ex, "Error En For Principal " + detalle.pServicioRef);
                    }
                }
                return entidad;
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogErrorAuto(ex);
                throw;
            }
        }


        public Entidades.clsEDebitoAuto mtdInsertar(Entidades.clsEDebitoAuto pEntidad)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                var strSql = "GANADERO.SPR_RCT_DEB_AUTO_MAE";
                var listaParametros = new List<OracleParameter>();
                listaParametros.Add(new OracleParameter("PCOD_CONVENIO", OracleDbType.Int32, pEntidad.pCodConvenio, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("PCOD_PLAZA", OracleDbType.Int32, pEntidad.pCodPlaza, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("PFECHA", OracleDbType.Date, pEntidad.pFecha, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("PUSUARIO", OracleDbType.Varchar2, pEntidad.pUsuario, ParameterDirection.Input));
                listaParametros.Add(new OracleParameter("pDataSet", OracleDbType.RefCursor, ParameterDirection.Output));
                var dataSet = db.mtdEjecutarSPconRetorno(listaParametros, strSql);
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    throw new Exception("Error al insertar, no se devolvio ningun resultado.");

                return mtdMapToEntidad(dataSet.Tables[0].Rows[0]);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw;
            }
        }

        public void mtdInsertarDetalle(Entidades.clsEDebitoAutoDet pEntidad, bool pEsActualizar = false)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                var listaParametros = new List<OracleParameter>();
                var strSql = @"INSERT INTO GANADERO.RCT_DEB_AUTO_DET
                                (COD_MAE, NRO_CONSULTA, SERVICIO_REFERENCIA, JTS_OID_SALDO, ESTADO, DESC_ERROR)
                                VALUES({0}, {1}, '{2}', {3}, '{4}', '{5}')";
                if (pEsActualizar)
                {
                    strSql = @"UPDATE GANADERO.RCT_DEB_AUTO_DET
                                SET NRO_CONSULTA = {1}, JTS_OID_SALDO = {3}, ESTADO = '{4}', DESC_ERROR = '{5}'
                                WHERE COD_MAE = {0} AND SERVICIO_REFERENCIA = '{2}'";
                }
                strSql = string.Format(strSql, pEntidad.pCodigoMae, pEntidad.pNroConsulta,
                    pEntidad.pServicioRef, pEntidad.pJtsOidSaldo, pEntidad.pEstado, pEntidad.pDescError);
                if (db.mtdEjecutarComandoTxt(listaParametros, strSql) == -1)
                    clsNUtils.escribirLogError("error al insertar, no se actualizado nada.", "clsNDebitoAuto.mtdInsertarDetalle");
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw;
            }
        }

        public Entidades.clsEDebitoAuto mtdObtener(int pCodigo)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                var sql = string.Format("Select * from GANADERO.RCT_DEB_AUTO_MAE where CODIGO = {0}", pCodigo);
                var dataSet = db.mtdEjecutarConsulta(sql);
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    throw new Exception("Error no se encontro ningun resultado.");
                return mtdMapToEntidad(dataSet.Tables[0].Rows[0]);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw;
            }
        }

        public Entidades.clsEDebitoAuto mtdObtener2(int pCodigo)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                var sql = string.Format("Select * from GANADERO.RCT_DEB_AUTO_MAE where ROWNUM =1", pCodigo);
                var dataSet = db.mtdEjecutarConsulta(sql);
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    throw new Exception("Error no se encontro ningun resultado.");
                return mtdMapToEntidad(dataSet.Tables[0].Rows[0]);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw;
            }
        }

        public List<Entidades.clsEDebitoAutoDet> mtdObtenerDetalle(int pCodigoMae)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                /*var sql = string.Format("Select * from RCT_DEB_AUTO_DET where COD_MAE = {0} ORDER BY SERVICIO_REFERENCIA ASC", pCodigoMae);*/
                var sql = string.Format("Select * from GANADERO.RCT_DEB_AUTO_DET where COD_MAE IN (SELECT CODIGO FROM GANADERO.RCT_DEB_AUTO_MAE WHERE TRUNC(FECHA) = (SELECT FECHAPROCESO FROM GANADERO.PARAMETROS )) ORDER BY TRIM(SERVICIO_REFERENCIA) ASC", pCodigoMae);
                var dataSet = db.mtdEjecutarConsulta(sql);
                var lista = new List<Entidades.clsEDebitoAutoDet>();
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    return lista;

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var entidad = mtdMapToEntidadDet(row);
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

        public List<Entidades.clsEDebitoAutoDet> mtdObtenerDetalleXUno(int pCodigoMae,int pNroConsulta)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                /*var sql = string.Format("Select * from RCT_DEB_AUTO_DET where COD_MAE = {0} ORDER BY SERVICIO_REFERENCIA ASC", pCodigoMae);*/
                var sql = string.Format("Select * from GANADERO.RCT_DEB_AUTO_DET where COD_MAE IN (SELECT CODIGO FROM GANADERO.RCT_DEB_AUTO_MAE WHERE TRUNC(FECHA) = (SELECT FECHAPROCESO FROM GANADERO.PARAMETROS )) AND NRO_CONSULTA = {0} ORDER BY TRIM(SERVICIO_REFERENCIA) ASC", pNroConsulta);
                var dataSet = db.mtdEjecutarConsulta(sql);
                var lista = new List<Entidades.clsEDebitoAutoDet>();
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    return lista;

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var entidad = mtdMapToEntidadDet(row);
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

        private Entidades.clsEDebitoAuto mtdMapToEntidad(DataRow dataRow)
        {
            if (dataRow == null) return null;
            var entidad = new Entidades.clsEDebitoAuto();
            entidad.pCodigo = Convert.ToInt32(dataRow["CODIGO"]);
            entidad.pCodConvenio = Convert.ToInt32(dataRow["COD_CONVENIO"]);
            entidad.pCodPlaza = Convert.ToInt32(dataRow["COD_PLAZA"]);
            entidad.pFecha = Convert.ToDateTime(dataRow["FECHA"]);
            entidad.pUsuario = Convert.ToString(dataRow["USUARIO"]);
            return entidad;
        }

        private Entidades.clsEDebitoAutoDet mtdMapToEntidadDet(DataRow dataRow)
        {
            if (dataRow == null) return null;
            var entidad = new Entidades.clsEDebitoAutoDet();
            entidad.pCodigoMae = Convert.ToInt32(dataRow["COD_MAE"]);
            entidad.pNroConsulta = Convert.ToInt32(dataRow["NRO_CONSULTA"]);
            entidad.pServicioRef = Convert.ToString(dataRow["SERVICIO_REFERENCIA"]);
            entidad.pJtsOidSaldo = Convert.ToInt32(dataRow["JTS_OID_SALDO"]);
            entidad.pEstado = Convert.ToString(dataRow["ESTADO"]);
            entidad.pDescError = Convert.ToString(dataRow["DESC_ERROR"]);
            return entidad;
        }
    }
}
