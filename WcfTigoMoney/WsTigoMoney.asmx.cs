using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WcfTigoMoney
{
    /// <summary>
    /// Descripción breve de WsTigoMoney
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WsTigoMoney : System.Web.Services.WebService
    {

        public string Login(string nombreBilletara, string nroPin)
        {
            try
            {
                var transId = new Negocio.clsNTigoMoneyWS().Login(nombreBilletara, nroPin);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: transId));
            }
            catch (Negocio.TigoMoneyException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pNameSpaceError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        [WebMethod]
        public string CargaBilletera(string pNroTran)
        {
            try
            {
                var transId = new Negocio.clsNTigoMoneyWS().CargarBilletera(pNroTran);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: transId));
            }
            catch (Negocio.TigoMoneyException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pNameSpaceError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        [WebMethod]
        public string RetiroBilletera(string pNroTran)
        {
            try
            {
                var transId = new Negocio.clsNTigoMoneyWS().DebitarBilletera(pNroTran);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: transId));
            }
            catch (Negocio.TigoMoneyException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pNameSpaceError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        [WebMethod]
        public string RetiroCupon(string pNroTran)
        {
            try
            {
                var transId = new Negocio.clsNTigoMoneyWS().DebitarBilleteraPorCupon(pNroTran);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: transId));
            }
            catch (Negocio.TigoMoneyException ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.pNameSpaceError, ex.pCodError));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        #region UTILIDADES

        public static string ConvertirAXml<T>(T pObjeto)
        {
            return Negocio.clsNUtils.ConvertirAXml(pObjeto);
        }

        #endregion
    }
}
