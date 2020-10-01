using Nexus.Sql.ResultTable;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nexus.Sql
{
    public abstract class DB_Context
    {
        public Dictionary<string, object> parameter = new Dictionary<string, object>();
        public Dictionary<string, Parameter> procedureParameter = new Dictionary<string, Parameter>();
        public abstract Task AddParameter(object data);
        public abstract Task<SqlQuery> Create(object create);
        public abstract Task<SqlQuery> Update(object update);
        public abstract Task<SqlQuery> Delete(object delete);
        public abstract Task<object> Populate(ResultSet result, int cursor);
    }
}
