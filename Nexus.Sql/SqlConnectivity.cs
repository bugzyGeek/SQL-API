using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace Nexus.Sql
{
    internal class SqlConnectivity : IDisposable
    {
        internal SqlConnection con;
        internal SqlCommand cmd;
        internal SqlTransaction transaction
        {
            get;
            set;
        }
        public SqlConnectivity(string connectionString) : this(new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString].ConnectionString))
        {
        }
        public SqlConnectivity(SqlConnection connection)
        {
            this.con = connection;
        }
        internal void EnsureConnectOpen()
        {
            if (this.con.State == ConnectionState.Open)
            {
                return;
            }
            this.con.Open();
        }
        internal void EnsureConnectClose()
        {
            if (this.con.State == ConnectionState.Open)
            {
                this.con.Close();
            }
        }
        public void Dispose()
        {
            if (this.con.State == ConnectionState.Open)
            {
                this.con.Close();
                this.con.Dispose();
            }
            else
            {
                if (this.con.State != ConnectionState.Open || this.con != null)
                {
                    this.con.Dispose();
                }
            }
            this.con = null;
        }
    }
}
