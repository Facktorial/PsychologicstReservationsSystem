using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class IdWrapper<T> : IEntity
        // , IHaveIndexer
        where T : new()
    {
        public IdWrapper() { throw new Exception("This constructor should not be used. Use the other constructor."); }
        public IdWrapper(T obj) { _obj = obj; Id = -1; }
        public IdWrapper(T obj, int id) { _obj = obj; Id = id; }
        public T _obj { get; set; }
        public int Id { get; set; }

        public string ToString() { return $"{typeof(T).Name}:  Id: {Id}, {_obj.ToString()}"; }

        private dynamic AsDynamic() => _obj;
    }
}
