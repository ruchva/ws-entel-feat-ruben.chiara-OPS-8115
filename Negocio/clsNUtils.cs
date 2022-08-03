using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Negocio
{
    public class clsNUtils
    {

        public static void escribirLogInfo(object pObject, string pSource)
        {
            try
            {
                Datos.clsLog.mtdLogInfo(JsonConvert.SerializeObject(pObject), pSource);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
            }
        }

        public static void escribirLogInfoAuto(object pObject, string pSource)
        {
            try
            {
                Datos.clsLog.mtdLogInfoAuto(JsonConvert.SerializeObject(pObject), pSource);
            }
            catch (Exception ex)
            {
                clsNUtils.escribirLogError(ex);
            }
        }

        public static void escribirLogInfo(string pMensaje, string pSource)
        {
            Datos.clsLog.mtdLogInfo(pMensaje, pSource);
        }
        public static void escribirLogInfoAuto(string pMensaje, string pSource)
        {
            Datos.clsLog.mtdLogInfoAuto(pMensaje, pSource);
        }
        public static void escribirLogError(string pMensaje, string pSource)
        {
            Datos.clsLog.mtdLogError(pMensaje, pSource);
        }

        public static void escribirLogError(Exception ex, string pMensaje = "")
        {
            Datos.clsLog.mtdLogError(ex, pMensaje);
        }

        public static void escribirLogErrorAuto(Exception ex, string pMensaje = "")
        {
            Datos.clsLog.mtdLogErrorAuto(ex, pMensaje);
        }
        public static string ConvertirAXml<T>(T pObjeto)
        {
            XmlSerializer lXslSerializer;
            MemoryStream lMstDato = new MemoryStream();
            XmlTextWriter lXtxWriter;
            string lStrResp;

            lXslSerializer = new XmlSerializer(pObjeto.GetType());
            lXtxWriter = new XmlTextWriter(lMstDato, Encoding.GetEncoding("ISO-8859-1"));

            lXslSerializer.Serialize(lXtxWriter, pObjeto);
            lMstDato = (MemoryStream)lXtxWriter.BaseStream;

            lStrResp = Encoding.GetEncoding("ISO-8859-1").GetString(lMstDato.ToArray());

            lMstDato.Close();
            lXtxWriter.Close();

            lMstDato.Dispose();

            return lStrResp;
        }
    }
}
