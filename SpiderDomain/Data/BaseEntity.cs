using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderDomain.Data
{
    public class BaseEntity : IEntity
    {
        public int id { get; set; }

        public DateTime lastdate { get; set; }
    }
}
