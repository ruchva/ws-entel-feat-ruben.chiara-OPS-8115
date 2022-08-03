using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net;
using System.IO;
using System.Configuration;
using System.Security.Cryptography;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Negocio
{
    public class clsNTigoMoneyWS
    {
        private string gErrorServicioTigo = "Error de conexion con Tigo, intente nuevamente.";
        private string gUrl;
        //private string gNombreBilletera;
        //private string gNroPIN;

        public clsNTigoMoneyWS()
        {
            this.gUrl = ConfigurationManager.AppSettings["Tigo_Url"];
            //this.gNombreBilletera = ConfigurationManager.AppSettings["Nombre_Billetera"];
            //this.gNroPIN = ConfigurationManager.AppSettings["Nro_PIN"];
        }

        public string CrearSession()
        {
            try
            {
                var metodo = "createsession";
                var datos = "<urn:createsession/>";
                var response = CallWebService(metodo, datos);
                var data = ProcessResponse(metodo, response);
                return data["sessionid"];
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw;
            }
        }

        public string CrearHash(string sessionId, string nombreBilletera, string nroPin)
        {
            try
            {
                var hash1 = Hash_SHA1(nombreBilletera.ToLower() + nroPin);
                var hash2 = Hash_SHA1(sessionId + hash1);
                return hash2.ToUpper();
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw new Exception("Error al generar codigo hash.");
            }
        }

        public string Login(string nombreBilletera, string nroPin, string pSessionId = null, string pHash = null)
        {
            try
            {
                var sessionId = pSessionId ?? CrearSession();
                var hash = pHash ?? CrearHash(sessionId, nombreBilletera, nroPin);
                var metodo = "login";
                var datos = string.Format(
                           @"<urn:login>
                                <urn:loginRequest>
                                    <urn:sessionid>{0}</urn:sessionid>
                                    <urn:initiator>{1}</urn:initiator>
                                    <urn:pin>{2}</urn:pin>
                                </urn:loginRequest>
                            </urn:login>", sessionId, nombreBilletera, hash);

                var response = CallWebService(metodo, datos);
                var data = ProcessResponse(metodo, response);
                return data["transid"];
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw;
            }
        }

        public string CargarBilletera(string pNroTran)
        {
            var sessionId = "";
            var nroTransIdLogin = "";
            var nroTransId = "";
            var hash = "";
            try
            {
                var tigo = ObtenerTranTigo(pNroTran);
                if (tigo.pEstado != 1) // 1 = Pendiente
                    throw new Exception("Error la transaccion tiene un estado no permitido para este metodo.");
                sessionId = CrearSession();
                hash = CrearHash(sessionId, tigo.pNomBilleteraConexion, tigo.pNroPinConexion);
                nroTransIdLogin = Login(tigo.pNomBilleteraConexion, tigo.pNroPinConexion, sessionId, hash);
                var metodo = "domesticRemit";
                var datos = @"
                <urn:domesticRemit>
                    <urn:domesticRemitRequest>
                        <std:extra_trans_data>
                            <misc:keyValuePairs>
                                <misc:key>details</misc:key>
                                <misc:value>{0}</misc:value>
                            </misc:keyValuePairs>
                            <misc:keyValuePairs>
                                <misc:key>national_id</misc:key>
                                <misc:value>{1}</misc:value>
                            </misc:keyValuePairs>
                        </std:extra_trans_data>
                        <urn:sessionid>{2}</urn:sessionid>
                        <urn:to>{3}</urn:to>
                        <urn:amount>{4}</urn:amount>
                    </urn:domesticRemitRequest>
                </urn:domesticRemit>";
                datos = string.Format(datos,
                    tigo.pDetails, tigo.pNationalId, sessionId, tigo.pNroTelefono, tigo.pMtoCarga);
                var response = CallWebService(metodo, datos);
                var data = ProcessResponse(metodo, response);
                nroTransId = data["transid"];
                ActualizarTranTigo(sessionId, hash, nroTransIdLogin, nroTransId, 2, "0", "", pNroTran);
                return nroTransId;
            }
            catch (System.Net.WebException ex)
            {
                clsNUtils.escribirLogError(ex);
                throw new Exception(this.gErrorServicioTigo);
            }
            catch (TigoMoneyException ex)
            {
                clsNUtils.escribirLogError(ex);
                ActualizarTranTigo(sessionId, hash, nroTransIdLogin, nroTransId, 1, ex.pCodError, ex.pNameSpaceError, pNroTran);
                throw;
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw;
            }
        }

        public string DebitarBilletera(string pNroTran)
        {
            var sessionId = "";
            var nroTransIdLogin = "";
            var nroTransId = "";
            var hash = "";
            try
            {
                var tigo = ObtenerTranTigo(pNroTran);
                if (tigo.pEstado != 1) // 1 = Pendiente
                    throw new Exception("Error la transaccion tiene un estado no permitido para este metodo.");
                sessionId = CrearSession();
                hash = CrearHash(sessionId, tigo.pNomBilleteraConexion, tigo.pNroPinConexion);
                nroTransIdLogin = Login(tigo.pNomBilleteraConexion, tigo.pNroPinConexion, sessionId, hash);
                var metodo = "cashout";
                var datos = @"
                <urn:cashoutRequest>
                    <urn:cashoutRequestType>
                        <std:extra_trans_data>
                            <misc:keyValuePairs>
                                <misc:key>national_id</misc:key>
                                <misc:value>{0}</misc:value>
                            </misc:keyValuePairs>
                        </std:extra_trans_data>
                        <urn:sessionid>{1}</urn:sessionid>
                        <urn:to>{2}</urn:to>
                        <urn:amount>{3}</urn:amount>
                    </urn:cashoutRequestType>
                </urn:cashoutRequest>";
                datos = string.Format(datos,
                    tigo.pNationalId, sessionId, tigo.pNroTelefono, tigo.pMtoCarga);
                var response = CallWebService(metodo, datos);
                var data = ProcessResponse(metodo, response);
                nroTransId = data["transid"];
                ActualizarTranTigo(sessionId, hash, nroTransIdLogin, nroTransId, 2, "0", "", pNroTran);
                return nroTransId;
            }
            catch (System.Net.WebException ex)
            {
                clsNUtils.escribirLogError(ex);
                throw new Exception(this.gErrorServicioTigo);
            }
            catch (TigoMoneyException ex)
            {
                clsNUtils.escribirLogError(ex);
                ActualizarTranTigo(sessionId, hash, nroTransIdLogin, nroTransId, 1, ex.pCodError, ex.pNameSpaceError, pNroTran);
                throw;
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw;
            }
        }

        public string DebitarBilleteraPorCupon(string pNroTran)
        {
            var sessionId = "";
            var nroTransIdLogin = "";
            var nroTransId = "";
            var hash = "";
            try
            {
                var tigo = ObtenerTranTigo(pNroTran);
                if (tigo.pEstado != 1) // 1 = Pendiente
                    throw new Exception("Error la transaccion tiene un estado no permitido para este metodo.");
                sessionId = CrearSession();
                hash = CrearHash(sessionId, tigo.pNomBilleteraConexion, tigo.pNroPinConexion);
                nroTransIdLogin = Login(tigo.pNomBilleteraConexion, tigo.pNroPinConexion, sessionId, hash);
                var metodo = "coupontransfer";
                var datos = @"
                <urn:coupontransfer>
                    <urn:coupontransferRequestType>
                        <std:extra_trans_data>
                            <misc:keyValuePairs>
                                <misc:key>national_id</misc:key>
                                <misc:value>{0}</misc:value>
                            </misc:keyValuePairs>
                        </std:extra_trans_data>
                        <urn:sessionid>{1}</urn:sessionid>
                        <urn:couponid>{2}</urn:couponid>
                    </urn:coupontransferRequestType>
                </urn:coupontransfer>";
                datos = string.Format(datos,
                    tigo.pNationalId, sessionId, tigo.pNroCupon);
                var response = CallWebService(metodo, datos);
                var data = ProcessResponse(metodo, response);
                nroTransId = data["transid"];
                ActualizarTranTigo(sessionId, hash, nroTransIdLogin, nroTransId, 2, "0", "", pNroTran);
                return nroTransId;
            }
            catch (System.Net.WebException ex)
            {
                clsNUtils.escribirLogError(ex);
                throw new Exception(this.gErrorServicioTigo);
            }
            catch (TigoMoneyException ex)
            {
                clsNUtils.escribirLogError(ex);
                ActualizarTranTigo(sessionId, hash, nroTransIdLogin, nroTransId, 1, ex.pCodError, ex.pNameSpaceError, pNroTran);
                throw;
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw;
            }
        }



        public Entidades.clsETigoMoney ObtenerTranTigo(string pNroTran)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                var sql = string.Format("Select * from BMV_TIGO_MOVIMIENTOS where NRO_TRANSACCION = {0}", pNroTran);
                var dataSet = db.mtdEjecutarConsulta(sql);
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    throw new Exception("Error no se encontro ningun resultado.");
                return MapToEntidad(dataSet.Tables[0].Rows[0]);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw;
            }
        }

        public void ActualizarTranTigo(string sessionId, string hash, string transIdLogin, string transId, int estado, string codigoError, string namespacerError, string nroTrans)
        {
            try
            {
                var db = new Datos.clsOracleConexion();
                var listaParametros = new List<OracleParameter>();
                var sql = @"UPDATE GANADERO.BMV_TIGO_MOVIMIENTOS 
                            SET ESTADO={0}, NROSESSION='{1}', VALOR_HASH='{2}', TRANSIDLOGIN='{3}', TRANSID='{4}', RESULT_WS='{5}'
                            WHERE NRO_TRANSACCION='{6}'";
                sql = string.Format(sql, estado, sessionId, hash, transIdLogin, transId,
                    codigoError + "|" + namespacerError, nroTrans);
                clsNUtils.escribirLogInfo(sql, "clsNTigoMoneyWS.ActualizarTranTigo");
                if (db.mtdEjecutarComandoTxt(listaParametros, sql) == -1)
                    clsNUtils.escribirLogError("Error al insertar, no se ha actualizado nada.", "clsNTigoMoneyWS.ActualizarTranTigo");
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
                throw;
            }
        }

        public Entidades.clsETigoMoney MapToEntidad(DataRow pDataRow)
        {
            if (pDataRow == null) return null;
            var entidad = new Entidades.clsETigoMoney();
            entidad.pNroTransaccion = Convert.ToInt32(pDataRow["NRO_TRANSACCION"]);
            entidad.pNroAsiento = Convert.ToInt32(pDataRow["NRO_ASIENTO"]);
            entidad.pFechaProceso = Convert.ToDateTime(pDataRow["FECHA_PROCESO"]);
            entidad.pHoraProceso = Convert.ToString(pDataRow["HORA_PROCESO"]);
            entidad.pSucProceso = Convert.ToInt32(pDataRow["SUC_PROCESO"]);
            entidad.pUsuProceso = Convert.ToString(pDataRow["USU_PROCESO"]);
            entidad.pNroTelefono = Convert.ToInt64(pDataRow["NRO_TELEFONO"]);
            entidad.pCodMoneda = Convert.ToInt32(pDataRow["COD_MONEDA"]);
            entidad.pMtoCarga = Convert.ToDecimal(pDataRow["MTO_CARGA"]);
            entidad.pMtoComision = Convert.ToDecimal(pDataRow["MTO_COMISION"]);
            entidad.pCodPersona = Convert.ToInt64(pDataRow["COD_PERSONA"]);
            entidad.pEstado = Convert.ToInt32(pDataRow["ESTADO"]);
            entidad.pNomBilleteraConexion = Convert.ToString(pDataRow["NOM_BILLETERA_CONEXION"]);
            entidad.pNroPinConexion = Convert.ToString(pDataRow["NRO_PIN_CONEXION"]);
            entidad.pNationalId = Convert.ToString(pDataRow["NATIONAL_ID"]);
            entidad.pTipoTran = Convert.ToString(pDataRow["TIPO_TRAN"]);
            entidad.pCodPlaza = Convert.ToInt32(pDataRow["COD_PLAZA"]);
            entidad.pDetails = Convert.ToString(pDataRow["DETAILS"]);
            entidad.pNroSession = Convert.ToString(pDataRow["NROSESSION"]);
            entidad.pValorHash = Convert.ToString(pDataRow["VALOR_HASH"]);
            entidad.pTransIdLogin = Convert.ToString(pDataRow["TRANSIDLOGIN"]);
            entidad.pTransId = Convert.ToString(pDataRow["TRANSID"]);
            entidad.pResultWs = Convert.ToString(pDataRow["RESULT_WS"]);
            entidad.pMtoComisionTigo = Convert.IsDBNull(pDataRow["MTO_COMISION_TIGO"]) ? 0 : Convert.ToDecimal(pDataRow["MTO_COMISION_TIGO"]);
            entidad.pNroCupon = Convert.ToString(pDataRow["NRO_CUPON"]);
            return entidad;
        }

        #region UTILIDADES

        private string CallWebService(string pMetodo, string pDatos)
        {
            var _url = this.gUrl;
            var _action = "urn:UMARKETSCWS/" + pMetodo;

            XmlDocument soapEnvelopeXml = CreateSoapRequest(pDatos);
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
            webRequest.Timeout = 240000; // 4 minutos
            webRequest.ServerCertificateValidationCallback += (sender, cert, chain, error) =>
            {
                return true;// cert.GetCertHashString() == "xxxxxxxxxxxxxxxx";
            };
            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
            // suspend this thread until call is complete
            asyncResult.AsyncWaitHandle.WaitOne();
            // get the response from the completed web request.
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
                clsNUtils.escribirLogInfo(soapResult, "clsNTigoWS.CallWebService.Respuesta");
                return soapResult;
            }
        }

        private HttpWebRequest CreateWebRequest(string url, string action)
        {
            clsNUtils.escribirLogInfo("Url: " + url + ", Metodo: " + action, "clsNTigoWS.CreateWebRequest");
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private XmlDocument CreateSoapRequest(string xmlBody)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            var xml = string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:UMARKETSCWS""
xmlns:std=""http://www.utiba.com/delirium/ws/StdQuery"" xmlns:misc=""http://www.utiba.com/delirium/ws/Misc"">
    <soapenv:Header/>
    <soapenv:Body>
        {0}
    </soapenv:Body>
</soapenv:Envelope>", xmlBody);
            clsNUtils.escribirLogInfo(xml, "clsNTigoWS.CreateSoapRequest.DatosEnvio");
            soapEnvelopeDocument.LoadXml(xml);
            return soapEnvelopeDocument;
        }

        private void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        public string Hash_SHA1(string pStrValor)
        {
            var sha = new SHA1CryptoServiceProvider();
            byte[] bytesToHash;
            var lStrHash = "";
            bytesToHash = System.Text.Encoding.ASCII.GetBytes(pStrValor);
            bytesToHash = sha.ComputeHash(bytesToHash);
            foreach (Byte b in bytesToHash)
            {
                lStrHash += b.ToString("x2");
            }
            return lStrHash;
        }

        private Dictionary<string, string> ProcessResponse(string metodo, string xmlResponse, bool validateResult = true)
        {
            var document = new XmlDocument();
            document.LoadXml(xmlResponse);

            var prefix = document.LastChild.LastChild.LastChild.Prefix;
            prefix = string.IsNullOrWhiteSpace(prefix) ? "" : (prefix + ":");

            var nodeList = document.GetElementsByTagName(prefix + metodo + "Return");
            var dic = new Dictionary<string, string>();
            foreach (XmlElement node in nodeList)
            {
                foreach (XmlElement nodeChild in node)
                {
                    dic.Add(nodeChild.LocalName, nodeChild.InnerText);
                }
            }

            if (validateResult && dic["result"] != "0")
                throw new TigoMoneyException(dic["result"], dic["result_namespace"]);

            return dic;
        }

        #endregion
    }

    public class TigoMoneyException : Exception
    {
        public string pNameSpaceError { get; set; }
        public string pCodError { get; set; }
        public TigoMoneyException(string cod, string nameSpace)
            : base("Error Tigo codigo: " + cod + ", namespace: " + nameSpace)
        {
            this.pCodError = cod;
            this.pNameSpaceError = nameSpace;
        }
    }
}
