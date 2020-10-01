using Nexus.Sql.ResultTable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Nexus.Sql
{
    public sealed class SqlPreparedStatement : IDisposable
    {
        private SqlConnectivity connection;
        public IsolationLevel SqlIsolationLevel
        {
            get;
            set;
        }
        public SqlPreparedStatement(string connectionKey)
        {
            this.connection = new SqlConnectivity(connectionKey);
        }
        public SqlPreparedStatement(SqlConnection connection)
        {
            this.connection = new SqlConnectivity(connection);
        }
        public async Task<int> ExecuteNonQuery(List<SqlQuery> query)
        {
            int outcome = 0;
            try
            {
                this.connection.EnsureConnectOpen();
                this.connection.transaction = this.connection.con.BeginTransaction();
                foreach (SqlQuery current in query)
                {
                    outcome = 0;
                    this.connection.cmd = this.CreateQuery(current.Query, current.Parameters);
                    this.connection.cmd.Transaction = this.connection.transaction;
                    outcome = this.connection.cmd.ExecuteNonQuery();
                }
                this.connection.transaction.Commit();
                this.connection.EnsureConnectClose();
            }
            catch (SqlException varDV0)
            {
                this.connection.transaction.Rollback();
                this.connection.EnsureConnectClose();
                outcome = 0;
                throw varDV0;
            }
            return outcome;
        }
        public async Task<int> ExecuteNonQuery(SqlQuery query)
        {
            int outcome = 0;
            try
            {
                this.connection.EnsureConnectOpen();
                this.connection.transaction = this.connection.con.BeginTransaction();
                outcome = 0;
                this.connection.cmd = this.CreateQuery(query.Query, query.Parameters);
                this.connection.cmd.Transaction = this.connection.transaction;
                outcome = this.connection.cmd.ExecuteNonQuery();
                this.connection.transaction.Commit();
                this.connection.EnsureConnectClose();
            }
            catch (SqlException varAD0)
            {
                this.connection.transaction.Rollback();
                this.connection.EnsureConnectClose();
                outcome = 0;
                throw varAD0;
            }
            return outcome;
        }
        public async Task<ResultSet> ExecuteQuery(SqlQuery sqlQuery)
        {
            ResultSet resultSet = null;
            SqlDataReader sqlDataReader = null;
            try
            {
                resultSet = new ResultSet();
                this.connection.EnsureConnectOpen();
                this.connection.cmd = this.CreateQuery(sqlQuery.Query, sqlQuery.Parameters);
                sqlDataReader = this.connection.cmd.ExecuteReader();
                int fieldCount = sqlDataReader.FieldCount;
                while (sqlDataReader.Read())
                {
                    for (int i = 0; i < fieldCount; i++)
                    {
                        resultSet.Add(sqlDataReader.GetName(i), (sqlDataReader[i] == DBNull.Value) ? null : sqlDataReader[i]);
                    }
                }
            }
            catch (SqlException varTK0)
            {
                this.connection.EnsureConnectClose();
                throw varTK0;
            }
            if (sqlDataReader == null || !sqlDataReader.IsClosed)
                sqlDataReader.Close();
            return resultSet;
        }
        public async Task<object> ExecuteScalar(SqlQuery query)
        {
            object outcome = null;
            try
            {
                this.connection.EnsureConnectOpen();
                this.connection.cmd = this.CreateQuery(query.Query, query.Parameters);
                outcome = this.connection.cmd.ExecuteScalar();
            }
            catch (SqlException varIM0)
            {
                this.connection.EnsureConnectClose();
                throw varIM0;
            }
            return outcome;
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
        public void Dispose()
        {
            this.connection.Dispose();
        }
    }
}
