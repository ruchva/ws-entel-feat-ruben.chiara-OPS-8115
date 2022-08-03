using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace WcfGanadero
{
    public class Service : IService
    {
        public string mtdBuscarClienteEntel(string codigoBusqueda, string codigoAcceso)
        {
            try
            {
                var entidad = new Negocio.clsNEntelWS().mtdBuscarCliente(codigoBusqueda, codigoAcceso);
                return ConvertirAXml(clsRespuesta.mtdCrearOk(pObjeto: entidad.pNroConsulta));
            }
            catch (Exception ex)
            {
                return ConvertirAXml(clsRespuesta.mtdCrearError(ex.Message));
            }
        }

        public Entidades.clsEEntel mtdObtenerConsultaEntel(int nroConsulta)
        {
            try
            {
                var entidad = new Negocio.clsNEntelWS().mtdObtenerConsulta(nroConsulta);
                return entidad;
                // return clsRespuesta.mtdCrearOk(pObjeto: entidad);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public List<Entidades.clsEEntelDetalle> mtdObtenerDetalleEntel(int nroConsulta)
        {
            try
            {
                var lista = new Negocio.clsNEntelWS().mtdObtenerDetalle(nroConsulta);
                return lista;
                // return clsRespuesta.mtdCrearOk(pObjeto: lista);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
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
