using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace Datos
{
    public class clsLog
    {
        private static Object thisLock = new Object();  

        public static void mtdLog(string pStrMessage, string pStrSource, string pStrClave){
            string lStrPath;
            string lStrHora;
            string lStrNombreArchivo;
            StreamWriter lSwtArchivo = null;
            try
            {
                lock (thisLock)
                {
                    lStrPath = ConfigurationManager.AppSettings["LogDatos"];
                    lStrNombreArchivo = DateTime.Now.ToString("yyyy-MM-dd");

                    // Verificar si existe una carpeta creada para la fecha actual y carpeta de los parámetros
                    if (!Directory.Exists(lStrPath))
                    {
                        Directory.CreateDirectory(lStrPath);
                    }

                    lStrHora = DateTime.Now.ToString("HH:mm:ss");
                    lStrPath += lStrNombreArchivo + ".txt";

                    if (File.Exists(lStrPath))
                    {
                        lSwtArchivo = File.AppendText(lStrPath);
                    }
                    else
                    {
                        lSwtArchivo = File.CreateText(lStrPath);
                    }

                    // Contenido del Archivo
                    lSwtArchivo.WriteLine("");
                    lSwtArchivo.WriteLine(lStrHora + ": " + pStrClave);
                    lSwtArchivo.WriteLine("        : " + pStrSource);
                    lSwtArchivo.WriteLine("        : " + pStrMessage);
                    lSwtArchivo.WriteLine("");

                    // Cierra el archivo
                    if (lSwtArchivo != null) lSwtArchivo.Close();
                }
            }
            catch (Exception ex)
            {
                if (lSwtArchivo != null) lSwtArchivo.Close();
            }
        }

        public static void mtdLogAuto(string pStrMessage, string pStrSource, string pStrClave)
        {
            string lStrPath;
            string lStrHora;
            string lStrNombreArchivo;
            StreamWriter lSwtArchivo = null;
            try
            {
                lock (thisLock)
                {
                    lStrPath = ConfigurationManager.AppSettings["LogDatos"];
                    lStrNombreArchivo = "AUTO"+DateTime.Now.ToString("yyyy-MM-dd");

                    // Verificar si existe una carpeta creada para la fecha actual y carpeta de los parámetros
                    if (!Directory.Exists(lStrPath))
                    {
                        Directory.CreateDirectory(lStrPath);
                    }

                    lStrHora = DateTime.Now.ToString("HH:mm:ss");
                    lStrPath += lStrNombreArchivo + ".txt";

                    if (File.Exists(lStrPath))
                    {
                        lSwtArchivo = File.AppendText(lStrPath);
                    }
                    else
                    {
                        lSwtArchivo = File.CreateText(lStrPath);
                    }

                    // Contenido del Archivo
                    lSwtArchivo.WriteLine("");
                    lSwtArchivo.WriteLine(lStrHora + ": " + pStrClave);
                    lSwtArchivo.WriteLine("        : " + pStrSource);
                    lSwtArchivo.WriteLine("        : " + pStrMessage);
                    lSwtArchivo.WriteLine("");

                    // Cierra el archivo
                    if (lSwtArchivo != null) lSwtArchivo.Close();
                }
            }
            catch (Exception ex)
            {
                if (lSwtArchivo != null) lSwtArchivo.Close();
            }
        }
        public static void mtdLogInfo(string pStrMensaje, string pStrSource)
        {
            clsLog.mtdLog(pStrMensaje, pStrSource, "INFO");
        }

        public static void mtdLogInfoAuto(string pStrMensaje, string pStrSource)
        {
            clsLog.mtdLogAuto(pStrMensaje, pStrSource, "INFO");
        }
        public static void mtdLogError(string pStrMensaje, string pStrSource)
        {
            clsLog.mtdLog(pStrMensaje, pStrSource, "ERROR");
        }

        public static void mtdLogErrorAuto(string pStrMensaje, string pStrSource)
        {
            clsLog.mtdLogAuto(pStrMensaje, pStrSource, "ERROR");
        }
        public static void mtdLogErrorAuto(Exception ex, string pStrMensaje = "")
        {
            clsLog.mtdLogErrorAuto(pStrMensaje + " - " + ex.Message + "\n\r" + ex.StackTrace, ex.Source);
        }

        public static void mtdLogError(Exception ex, string pStrMensaje = "")
        {
            clsLog.mtdLogError(pStrMensaje + " - " + ex.Message + "\n\r" + ex.StackTrace, ex.Source);
        }
    }
}
