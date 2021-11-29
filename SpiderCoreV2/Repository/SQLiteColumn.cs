using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore.Repository
{
    public class SQLiteColumn
    {
        public SQLiteColumn(string name, string type)
        {
            this.ColumnName = name;
            this.DataType = type;
            this.Length = 0;
        }

        public SQLiteColumn(string name, string type, int length)
        {
            this.ColumnName = name;
            this.DataType = type;
            this.Length = length;
        }

        public string ColumnName { get; set; }

        public string DataType { get; set; }

        public int Length { get; set; }
    }
}
