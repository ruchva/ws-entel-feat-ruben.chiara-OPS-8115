using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;

namespace Datos
{
    public class clsOracleConexion
    {
        #region Atributos

        private string strConexion;

        OracleConnection guConection;

        #endregion

        #region Metodos

        #region Constructor

        public clsOracleConexion()
        {
            //string strDB = ConfigurationManager.AppSettings["DB"];
            strConexion = new clsCadenaConexion().Cadena;
        }

        public clsOracleConexion(string strDB)
        {
            strConexion = ConfigurationManager.AppSettings[strDB];
        }

        #endregion

        #region Getters


        public int mtdEjecutarComandoTxt(List<OracleParameter> parametros, string comandoSQL)
        {
            int intRe=0;
            try
            {
                using (OracleConnection conProxy = new OracleConnection(strConexion))
                {
                    conProxy.Open();
                    OracleCommand cmdProxy = new OracleCommand();
                    cmdProxy = conProxy.CreateCommand();
                    cmdProxy.CommandText = comandoSQL;
                    cmdProxy.CommandType = CommandType.Text;
                    cmdProxy.Parameters.AddRange(parametros.ToArray());
                    intRe = cmdProxy.ExecuteNonQuery();
                    conProxy.Close();
                    return intRe; 
                }
            }
            catch (Exception ex)
            {
                clsLog.mtdLogError(ex);
                throw ex;
            }
        }

        public DataSet mtdEjecutarConsulta(string sql)
        {
            try
            {
                DataSet dtsProxy = new DataSet();
                using (OracleConnection conProxy = new OracleConnection(strConexion))
                {
                    using (OracleDataAdapter adaProxy = new OracleDataAdapter(sql, conProxy))
                    {
                        conProxy.Open();
                        adaProxy.Fill(dtsProxy);
                        conProxy.Close();
                    }
                    return dtsProxy;
                }
            }
            catch (Exception ex)
            {
                clsLog.mtdLogError(ex);
                throw ex;
            }
        }
        /*
        public DataSet EjecutarConsulta(string sql, int longSize)
        {
            try
            {
                DataSet dtsProxy = new DataSet();
                using (OracleConnection conProxy = new OracleConnection(strConexion))
                {
                    using (OracleDataAdapter adaProxy = new OracleDataAdapter(sql, conProxy))
                    {
                        conProxy.Open();
                        adaProxy.SelectCommand.InitialLONGFetchSize = longSize;
                        adaProxy.SelectCommand.AddRowid = true;
                        adaProxy.Fill(dtsProxy);
                        return dtsProxy;
                    }
                }
            }
            catch (Exception ex)
            {
                clsLog.mtdLogError(ex);
                throw ex;
            }
        }
        */
        public DataSet EjecutarConsulta(List<DbParameter> parametros, string sql)
        {
            try
            {
                DataSet dtsProxy = new DataSet();
                using (OracleConnection conProxy = new OracleConnection(strConexion))
                {
                    using (OracleDataAdapter adaProxy = new OracleDataAdapter(sql, conProxy))
                    {
                        adaProxy.SelectCommand = CrearCommando(parametros, conProxy, sql);
                        conProxy.Open();
                        adaProxy.Fill(dtsProxy);
                        return dtsProxy;
                    }
                }
            }
            catch (Exception ex)
            {
                clsLog.mtdLogError(ex);
                throw ex;
            }
        }

        public DataSet mtdEjecutarSPconRetorno(List<OracleParameter> parametros, string comandoSQL)
        {
            try
            {
                DataSet dtsProxy = new DataSet();
                using (OracleConnection conexion = new OracleConnection(strConexion))
                {
                    OracleCommand cmdProxy = conexion.CreateCommand();
                    cmdProxy.CommandText = comandoSQL;
                    cmdProxy.CommandType = CommandType.StoredProcedure;
                    cmdProxy.Parameters.AddRange(parametros.ToArray());
                    using (OracleDataAdapter adaProxy = new OracleDataAdapter())
                    {
                        adaProxy.SelectCommand = cmdProxy;
                        adaProxy.Fill(dtsProxy);
                    }
                }
                return dtsProxy;
            }
            catch (Exception ex)
            {
                clsLog.mtdLogError(ex);
                throw ex;
            }
        }

        public bool mtdEjecutarSPAsincrono(List<OracleParameter> parametros, string comandoSQL)
        {
            try
            {
                DataSet dtsProxy = new DataSet();
                bool bRetorno = false;
                using (OracleConnection conexion = new OracleConnection(strConexion))
                {
                    OracleCommand cmdProxy = conexion.CreateCommand();
                    cmdProxy.CommandText = comandoSQL;
                    cmdProxy.CommandType = CommandType.StoredProcedure;
                    cmdProxy.Parameters.AddRange(parametros.ToArray());
                    conexion.Open();
                    bRetorno = (cmdProxy.ExecuteNonQueryAsync()).IsCompleted;
                    return bRetorno;
                }
            }
            catch (Exception ex)
            {
                clsLog.mtdLogError(ex);
                throw ex;
            }
        }

        public int mtdEjecutarSP(List<OracleParameter> parametros, string comandoSQL)
        {
            try
            {
                using (OracleConnection conexion = new OracleConnection(strConexion))
                {
                    OracleCommand cmdProxy = conexion.CreateCommand();
                    cmdProxy.CommandText = comandoSQL;
                    cmdProxy.CommandType = CommandType.StoredProcedure;
                    cmdProxy.Parameters.AddRange(parametros.ToArray());
                    conexion.Open();
                    return cmdProxy.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                clsLog.mtdLogError(ex);
                throw ex;
            }
        }

        #endregion

        #region Metodos Colaboracion

        private OracleCommand CrearCommando(List<DbParameter> parametros, OracleConnection conProxy, string sql)
        {
            OracleCommand cmdProxy = conProxy.CreateCommand();
            cmdProxy.CommandType = CommandType.StoredProcedure;
            cmdProxy.CommandText = sql;

            foreach (DbParameter parametro in parametros)
            {
                cmdProxy.Parameters.Add((OracleParameter)parametro);
            }
            return cmdProxy;
        }

        #endregion

        #endregion
    }
}
