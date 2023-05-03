using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    internal class AssociativeTableMapping<TEntity1, TEntity2>
        where TEntity1 : class
        where TEntity2 : class
    {
        public int IdEntity1 { get; set; }
        public int IdEntity2 { get; set; }
        public TEntity1 Entity1 { get; set; }
        public TEntity2 Entity2 { get; set; }
    }
}
