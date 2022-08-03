using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Datos
{
    public class clsCadenaConexion
    {
        private string cadena;

        public string Servidor { get; set; }
        public string BaseDatos { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }

        public clsCadenaConexion()
        {
            Servidor = ConfigurationManager.AppSettings["Servidor"];
            BaseDatos = ConfigurationManager.AppSettings["BDatos"];
            Usuario = ConfigurationManager.AppSettings["Usuario"];
            Password = ConfigurationManager.AppSettings["Password"];
        }

        public string Cadena
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(cadena)) return cadena;
                //var str = "Provider=MSDAORA.1;User ID= {2};Password={3};Data Source={0}{1};Persist Security Info=False"; 
                var str = "Data Source={0}{1};Persist Security Info=True;User ID={2};Password={3}";
                
                str = string.Format(str, this.Servidor, this.BaseDatos,
                    DecodificarPinTrans(this.Usuario),
                    DecodificarPinTrans(this.Password));
                cadena = str;
                return cadena;
            }
            set
            {
                cadena = value;
            }
        }


        private string DecodificarPinTrans(string pStrPina)
        {
            var lStrPin = "";
            var lStrCad = "";
            var lShrCont = 0;

            pStrPina = pStrPina.Trim();
            if (string.IsNullOrWhiteSpace(pStrPina)) return String.Empty;

            pStrPina += pStrPina[pStrPina.Length - 1] == '|' ? "" : "|";

            for (lShrCont = 1; lShrCont <= pStrPina.Length; lShrCont++)
            {
                if (pStrPina[lShrCont - 1] != '|')
                {
                    lStrCad += pStrPina[lShrCont - 1];
                }
                else
                {
                    var str = DecodificarPin(Convert.ToInt32(lStrCad));
                    lStrPin += Convert.ToChar(Convert.ToInt32(str));
                    lStrCad = "";
                }
            }

            return lStrPin;
        }

        private string DecodificarPin(int pIntPina)
        {
            string lStrAuxPin = "";
            string lStrBase = "";
            int lIntContI;
            int lIntContJ;
            string lStrPin = "";

            lStrAuxPin = pIntPina.ToString("0000");

            for (lIntContI = 1; lIntContI <= 4; lIntContI++)
            {
                switch (lIntContI)
                {
                    case 1:
                        lStrBase = "5761892304";
                        break;
                    case 2:
                        lStrBase = "9376524180";
                        break;
                    case 3:
                        lStrBase = "0493185267";
                        break;
                    case 4:
                        lStrBase = "7319248056";
                        break;
                }

                for (lIntContJ = 0; lIntContJ < 10; lIntContJ++)
                {
                    var a = lStrAuxPin.Substring(lIntContI - 1, 1);
                    var b = lStrBase.Substring(lIntContJ, 1);
                    if (a == b)
                    {
                        break;
                    }
                }

                if (lIntContJ >= 9)
                {
                    lIntContJ = 0;
                }
                else
                {
                    lIntContJ++;
                }

                lStrPin = lStrPin + lIntContJ.ToString("0");
            }

            return lStrPin;
        }
    }
}
