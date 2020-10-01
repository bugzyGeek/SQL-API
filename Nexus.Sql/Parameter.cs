using System;
using System.Data;
namespace Nexus.Sql
{
    public sealed class Parameter
    {
        public bool IsOut
        {
            get;
            set;
        }
        public SqlDbType OutType
        {
            get;
            set;
        }
        public object Value
        {
            get;
            set;
        }
        public int Size
        {
            get;
            set;
        }
    }
}
