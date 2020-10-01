using Nexus.Sql.ResultTable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Nexus.Sql
{
    public sealed class SqlStoreProcedure : IDisposable
    {
        private SqlConnectivity connection;
        private bool transactionBegan;
        public object this[string key]
        {
            get
            {
                return this.connection.cmd.Parameters[key].Value;
            }
        }
        internal Dictionary<string, SqlParameter> SqlParameters
        {
            get;
            private set;
        }
        public SqlStoreProcedure(string connection)
        {
            this.SqlParameters = new Dictionary<string, SqlParameter>();
            this.connection = new SqlConnectivity(connection);
            this.transactionBegan = false;
        }
        private SqlCommand CreateQuery(string cmd, Dictionary<string, Parameter> parameters)
        {
            SqlCommand varJJ = new SqlCommand(cmd, this.connection.con);
            varJJ.CommandType = CommandType.StoredProcedure;
            SqlCommand sqlCommand = varJJ;
            if (parameters != null)
            {
                sqlCommand.Parameters.AddRange( this.AddParameters(parameters));
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
                    Dictionary<string, SqlParameter> varVA0 = this.SqlParameters;
                    string varWI = current;
                    SqlParameter varIF = new SqlParameter(varWI, param[current].Value ?? "");
                    varIF.Direction = ParameterDirection.Output;
                    varIF.SqlDbType = param[current].OutType;
                    varIF.Size = param[current].Size;
                    varVA0.Add(varWI, varIF);
                    list.Add(this.SqlParameters[current]);
                }
                else
                {
                    list.Add(new SqlParameter(current, param[current].Value ?? ""));
                }
            }
            return list.ToArray();
        }
        public async Task<int> ExecuteNonQuery(List<SqlQuery> query)
        {
            int outcome = 0;
            try
            {
                this.connection.EnsureConnectOpen();
                foreach (SqlQuery current in query)
                {
                    outcome = 0;
                    this.connection.cmd = this.CreateQuery(current.Query, current.ProcdureParameters);
                    this.connection.cmd.Transaction = this.connection.transaction;
                    outcome = this.connection.cmd.ExecuteNonQuery();
                }
                this.SqlParameters.Clear();
            }
            catch (SqlException varBG0)
            {
                if (this.transactionBegan)
                {
                    this.connection.transaction.Rollback();
                }
                this.transactionBegan = false;
                this.connection.EnsureConnectClose();
                outcome = 0;
                throw varBG0;
            }
            return outcome;
        }
        public async Task<int> ExecuteNonQuery(SqlQuery query)
        {
            int outcome = 0;
            try
            {
                this.connection.EnsureConnectOpen();
                outcome = 0;
                this.connection.cmd = this.CreateQuery(query.Query, query.ProcdureParameters);
                this.connection.cmd.Transaction = this.connection.transaction;
                outcome = this.connection.cmd.ExecuteNonQuery();
            }
            catch (SqlException varMO0)
            {
                if (this.transactionBegan)
                {
                    this.connection.transaction.Rollback();
                }
                this.transactionBegan = false;
                this.connection.EnsureConnectClose();
                outcome = 0;
                throw varMO0;
            }
            this.SqlParameters.Clear();
            return outcome;
        }
        public async Task<ResultSet> ExecuteQuery(SqlQuery sqlQuery)
        {
            ResultSet resultSet = null;
            try
            {
                resultSet = new ResultSet();
                this.connection.cmd = this.CreateQuery(sqlQuery.Query, sqlQuery.ProcdureParameters);
                this.connection.cmd.Transaction = this.connection.transaction;
                using (SqlDataReader sqlDataReader = this.connection.cmd.ExecuteReader())
                {
                    int fieldCount = sqlDataReader.FieldCount;
                    while (sqlDataReader.Read())
                    {
                        for (int i = 0; i < fieldCount; i++)
                        {
                            resultSet.Add(sqlDataReader.GetName(i), (sqlDataReader[i] == DBNull.Value) ? null : sqlDataReader[i]);
                        }
                    }
                }
            }
            catch (SqlException varBE0)
            {
                this.connection.transaction.Rollback();
                this.connection.EnsureConnectClose();
                throw varBE0;
            }
            this.SqlParameters.Clear();
            return resultSet;
        }
        public async Task<object> ExecuteScalar(SqlQuery query)
        {
            object outcome = null;
            try
            {
                this.connection.cmd = this.CreateQuery(query.Query, query.ProcdureParameters);
                outcome = this.connection.cmd.ExecuteScalar();
            }
            catch (SqlException varWD0)
            {
                this.connection.EnsureConnectClose();
                throw varWD0;
            }
            this.SqlParameters.Clear();
            return outcome;
        }
        public void Dispose()
        {
            this.connection.Dispose();
        }
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            this.connection.EnsureConnectOpen();
            this.connection.transaction = this.connection.con.BeginTransaction(isolationLevel);
            this.transactionBegan = true;
        }
        public void CommitTransaction()
        {
            this.connection.transaction.Commit();
            this.connection.EnsureConnectClose();
        }
    }
}
