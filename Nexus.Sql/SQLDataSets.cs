using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Nexus.Sql
{
    public class SqlDataSets : IDisposable
    {
        private SqlConnectivity connection;
        private SqlDataAdapter dAdapter = new SqlDataAdapter();
        public Dictionary<string, SqlParameter> SqlParameters
        {
            get;
            private set;
        }
        public SqlTransaction Transaction
        {
            get;
            set;
        }
        public SqlDataSets(string connection)
        {
            this.SqlParameters = new Dictionary<string, SqlParameter>();
            this.connection = new SqlConnectivity(connection);
        }
        public SqlDataSets(SqlConnection connection)
        {
            this.SqlParameters = new Dictionary<string, SqlParameter>();
            this.connection = new SqlConnectivity(connection);
        }
        public async Task<DataTable> ExecutePreparedStatementQuery(SqlQuery sqlQuery, string tableName)
        {
            DataTable dataTable = new DataTable(tableName);
            try
            {
                this.connection.EnsureConnectOpen();
                this.connection.cmd = this.CreateQuery(sqlQuery.Query, sqlQuery.Parameters);
                this.dAdapter.SelectCommand = this.connection.cmd;
                this.dAdapter.FillSchema(dataTable, SchemaType.Source);
                this.dAdapter.Fill(dataTable);
            }
            catch (SqlException varOD0)
            {
                this.connection.EnsureConnectClose();
                throw varOD0;
            }
            return dataTable;
        }
        public async Task<DataTable> ExecuteStoredProcudureQuery(SqlQuery sqlQuery, string tableName)
        {
            DataTable dataTable = new DataTable(tableName);
            try
            {
                this.connection.cmd = this.CreateQuery(sqlQuery.Query, sqlQuery.ProcdureParameters);
                this.dAdapter.SelectCommand = this.connection.cmd;
                this.dAdapter.FillSchema(dataTable, SchemaType.Mapped);
                this.dAdapter.Fill(dataTable);
            }
            catch (SqlException varOD0)
            {
                this.Transaction.Rollback();
                this.connection.EnsureConnectClose();
                throw varOD0;
            }
            return dataTable;
        }
        private SqlCommand CreateQuery(string cmd, Dictionary<string, object> parameters)
        {
            SqlCommand varJJ = new SqlCommand(cmd, this.connection.con);
            varJJ.CommandType = CommandType.Text;
            SqlCommand sqlCommand = varJJ;
            if (parameters != null)
            {
                sqlCommand.Parameters.AddRange(this.AddParameters(parameters));
            }
            return sqlCommand;
        }
        private SqlParameter[] AddParameters(Dictionary<string, object> param)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            foreach (string current in param.Keys)
            {
                list.Add(new SqlParameter(current, param[current] ?? ""));
            }
            return list.ToArray();
        }
        private SqlCommand CreateQuery(string cmd, Dictionary<string, Parameter> parameters)
        {
            SqlCommand varJJ = new SqlCommand(cmd, this.connection.con);
            varJJ.CommandType = CommandType.StoredProcedure;
            SqlCommand sqlCommand = varJJ;
            if (parameters != null)
            {
                sqlCommand.Parameters.AddRange(this.AddParameters(parameters));
            }
            return sqlCommand;
        }
        private SqlParameter[] AddParameters(Dictionary<string, Parameter> param)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            foreach (string current in param.Keys)
            {
                if (param[current].IsOut)
                {
                    Dictionary<string, SqlParameter> varOM0 = this.SqlParameters;
                    string varWI = current;
                    SqlParameter varIF = new SqlParameter(varWI, param[current].Value ?? "");
                    varIF.Direction = ParameterDirection.Output;
                    varIF.SqlDbType = param[current].OutType;
                    varOM0.Add(varWI, varIF);
                    list.Add(this.SqlParameters[current]);
                }
                else
                {
                    list.Add(new SqlParameter(current, param[current].Value ?? ""));
                }
            }
            return list.ToArray();
        }
        public void Dispose()
        {
            this.connection.Dispose();
        }
    }
}
