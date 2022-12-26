using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SusQTech.Data.DataObjects
{
    public static class ConnectionFactory
    {
        private static string _connectionString = null;
        private static string _overrideConnectionString = string.Empty;

        public static SqlConnection GetConnection()
        {
            return (new SqlConnection(ConnectionFactory.DataObjectConnectionString));
        }

        public static void OverrideDefaultConnectionString(string ConnectionString)
        {
            ConnectionFactory._overrideConnectionString = ConnectionString;
        }

        public static string DataObjectConnectionString
        {
            get
            {
                //attempt to first return any override connection string
                if (!string.IsNullOrEmpty(ConnectionFactory._overrideConnectionString))
                    return (ConnectionFactory._overrideConnectionString);

                if (!string.IsNullOrEmpty(ConnectionFactory._connectionString))
                    return (ConnectionFactory._connectionString);

                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["DataObjectConnectionStringName"]))
                    throw new Exception("No connection string name specified in AppSettings:DataObjectConnectionStringName");

                if (ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DataObjectConnectionStringName"]] == null)
                    throw new Exception(string.Format("No connection string found with name {0}", ConfigurationManager.AppSettings["DataObjectConnectionStringName"]));

                ConnectionFactory._connectionString = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DataObjectConnectionStringName"]].ConnectionString;

                if (string.IsNullOrEmpty(ConnectionFactory._connectionString))
                    throw new Exception(string.Format("The connection string named {0} is empty.", ConfigurationManager.AppSettings["DataObjectConnectionStringName"]));

                return (ConnectionFactory._connectionString);
            }
        }
    }
}
