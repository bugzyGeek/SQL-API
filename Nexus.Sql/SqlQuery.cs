using System;
using System.Collections.Generic;
namespace Nexus.Sql
{
    public sealed class SqlQuery
    {
        public Dictionary<string, object> Parameters
        {
            get;
            set;
        }
        public Dictionary<string, Parameter> ProcdureParameters
        {
            get;
            set;
        }
        public string Query
        {
            get;
            set;
        }
    }
}
