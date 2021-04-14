using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockSimulateWeb.Base
{
    public class BaseModel
    {
        public int[] Ids { get; set; }
        public string Filter { get; set; }

        public string OrderBy { get; set; }

        public int TakeSize { get; set; }

        public string[] Columns { get; set; }

    }
}
