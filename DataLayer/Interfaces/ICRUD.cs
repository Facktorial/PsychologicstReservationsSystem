using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    internal interface ICRUD<T>
    {
        // because static
        // public T Find(int id, string connection);
        public void Save();
        public void Delete();
        public void Update();
    }
    internal interface ICRUDObj<T>
    {
        public T Find(int id);
        public void Save(T obj);
        public void Delete(T obj);
        public void Update(T obj);
    }

    internal interface ICRUDCollections<T>
    {
        // public T Find(int id, string connection);
        // public T Find(int id);
        public void Insert(T obj);
        public void Save();
        public void Delete(int i);
        public void Update(string propertyName, object value, string propertyChanging, object valueChanging);
    }

    internal interface ICRUDRepository<T> : ICRUDObj<T> where T : class
    {
        IEnumerable<T> GetAll();
    }
}
