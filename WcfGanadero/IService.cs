using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfGanadero
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebInvoke(
            Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            RequestFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "mtdBuscarClienteEntel?codigoBusqueda={codigoBusqueda}&codigoAcceso={codigoAcceso}")]
        string mtdBuscarClienteEntel(string codigoBusqueda, string codigoAcceso);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "mtdObtenerConsultaEntel?nroConsulta={nroConsulta}")]
        Entidades.clsEEntel mtdObtenerConsultaEntel(int nroConsulta);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            RequestFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "mtdObtenerDetalleEntel?nroConsulta={nroConsulta}")]
        List<Entidades.clsEEntelDetalle> mtdObtenerDetalleEntel(int nroConsulta);

    }


}
