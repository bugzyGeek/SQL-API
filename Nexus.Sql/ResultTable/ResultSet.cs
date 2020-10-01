using System;
using System.Collections.Generic;
using System.Linq;
namespace Nexus.Sql.ResultTable
{
    public sealed class ResultSet
    {
        private RowSet<string, List<object>> columns = new RowSet<string, List<object>>();
        public List<object> this[string columnName]
        {
            get
            {
                if (this.columns.ContainsKey(columnName))
                {
                    return this.columns[columnName];
                }
                return null;
            }
        }
        public int Count
        {
            get
            {
                return this.columns.Count;
            }
        }
        public int RowCount
        {
            get
            {
                if (this.columns.Values.Count<List<object>>() == 0)
                {
                    return 0;
                }
                return this.columns.Values.ToList<List<object>>()[0].Count;
            }
        }
        public IEnumerable<string> ColoumNames
        {
            get
            {
                return this.columns.ColumnNames;
            }
        }
        internal void Add(string columnName, object value)
        {
            if (this.columns.ContainsColumn(columnName))
            {
                this.columns[columnName].Add(value);
                return;
            }
            this.columns.Add(columnName, new List<object>
               {
                    value
               });
        }
    }
}
