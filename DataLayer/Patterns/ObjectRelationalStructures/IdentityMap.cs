using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class IdentityMap<T> where T : IEntity
    {
        private Dictionary<int, T> _entities = new Dictionary<int, T>();

        public T Get(int id)
        {
            if (_entities.ContainsKey(id)) {  return _entities[id]; }

            return default(T);
        }

        public void Add(T entity)
        {
            _entities.Add(entity.Id, entity);
        }
    }
}
