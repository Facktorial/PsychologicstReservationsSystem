using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public interface IHaveIndexer
    {
        public object this[string name] { get; set; }
        public Func<object[], object> this[Method methodName] { get; }
    }
}
