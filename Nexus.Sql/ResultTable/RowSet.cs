using System;
using System.Collections.Generic;
using System.Linq;
namespace Nexus.Sql.ResultTable
{
    internal sealed class RowSet<TKey, TValue> : Dictionary<string, List<object>>
    {
        public IEnumerable<List<object>> Values
        {
            get
            {
                return base.Values;
            }
        }
        public IEnumerable<string> ColumnNames
        {
            get
            {
                return base.Keys.ToList<string>();
            }
        }
        public new void Clear()
        {
            base.Clear();
        }
        public bool ContainsColumn(string column)
        {
            return base.ContainsKey(column);
        }
        public void Remove(string columnName)
        {
            base.Remove(columnName);
        }
    }
}
