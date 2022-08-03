using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfGanadero
{
    [DataContract]
    public class clsRespuesta
    {
        public const int OK = 0;
        public const int ERROR = 1;
        [DataMember]
        public string CODERROR { get; set; }
        [DataMember]
        public string DESERROR { get; set; }
        [DataMember]
        public int CODRESPUESTA { get; set; }
        [DataMember]
        public object NROEVENTO { get; set; }

        public clsRespuesta() { }

        public clsRespuesta(string pCodigoError, int pCodResp, string pMensaje, object pObjeto)
        {
            this.CODERROR = pCodigoError;
            this.CODRESPUESTA = pCodResp;
            this.DESERROR = pMensaje;
            this.NROEVENTO = pObjeto;
        }

        private static clsRespuesta init(string pCodigoError, int pCodResp, string pMensaje, object pObjeto)
        {
            return new clsRespuesta(pCodigoError, pCodResp, pMensaje, pObjeto);
        }

        public static clsRespuesta mtdCrearOk(string pMensaje = "Ok", string pCodError = "", object pObjeto = null)
        {
            return init(pCodError, OK, pMensaje, pObjeto);
        }

        public static clsRespuesta mtdCrearError(string pMensaje, string pCodError = "", object pObjeto = null)
        {
            return init(pCodError, ERROR, pMensaje, pObjeto);
        }
    }
}