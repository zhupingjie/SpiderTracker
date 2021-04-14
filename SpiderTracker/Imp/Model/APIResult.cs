using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp.Model
{
    public class APIResult
    {
        public bool Success => this.Code == 200;

        public int Code { get; set; } = 200;

        public string Message { get; set; }

        public Exception Error { get; set; }

        public object Result { get; set; }
    }
}
