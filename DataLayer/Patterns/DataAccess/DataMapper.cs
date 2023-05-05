using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace DataLayer
{
    public class DataMapper<T>
        : PatternPrototype<List<T>>, IHaveIndexer, IFetchable<DataMapper<T>>,
        IEnumerable<T>, 
        ICRUDCollections<T>
        where T : IEntity, new ()
    {
        StateOfCollection State;
        int MaxIdx;
        private UnitOfWork<List<T>, T> _unitUploadWork;

        public DataMapper(SqlConnector sql) : base()
        {
            SqlConnect = sql;
            Fetch();
            State = new StateOfCollection(
                new HashSet<int>(), new HashSet<int>(), new HashSet<int>()
            );
            _unitUploadWork = new UnitOfWork<List<T>, T>(DomainObject, SqlConnect);
        }
        public DataMapper(List<T> obj, SqlConnector sql) : base(null, -1, sql) 
        {
            Fetch();

            State = new StateOfCollection(
                new HashSet<int>(), new HashSet<int>(), new HashSet<int>()
            );

            foreach (var item in obj)
            {
                DomainObject.Add(item);
                State.Added.Add(DomainObject.Count - 1);
                Console.WriteLine("add: " + item);
            }

            _unitUploadWork = new UnitOfWork<List<T>, T>(DomainObject, SqlConnect);        
        }

        public DataMapper<T> Fetch()
        {
            MaxIdx = GetMaxId<T>();
            if (DomainObject != null) { DomainObject.Clear(); }
            else { DomainObject = new List<T>(); }

            Action<List<T>, T> fn = (ls, obj) => ls.Add(obj);

            InnerFetch<T>(fn);

            return this;
        }

        public T? Find(int id)
        {
            return DomainObject.SingleOrDefault(r => r.Id == id);
        }

        public List<T> FindByProperty(string propertyName, object value)
        {
            PropertyInfo prop = typeof(T).GetProperty(propertyName);

            return DomainObject.FindAll(t => prop.GetValue(t) == value);
        }

        public void Update(int index, T obj, int id)
        {
            if (index > 0 || index < DomainObject.Count)
            {
                DomainObject[index] = obj;
                DomainObject[index].Id = id;
                State.Changed.Add(id);
                State.Changed.Add(id);
                State.Added.Remove(id);
            }
        }

        public void Update(string propertyName, object value, string propertyChanging, object valueChanging)
        {
            PropertyInfo prop = typeof(T).GetProperty(propertyName);

            if (prop != null)
            {
                var toUpdate = DomainObject.SingleOrDefault(r => prop.GetValue(DomainObject) == value);
                if (toUpdate != null)
                {
                    PropertyInfo propChange = typeof(T).GetProperty(propertyChanging);
                    propChange.SetValue(toUpdate, valueChanging);
                    State.Changed.Add(toUpdate.Id);
                    State.Added.Remove(toUpdate.Id);
                }
            }
        }

        public void Update(string propertyName, object value, T obj)
        {
            PropertyInfo prop = typeof(T).GetProperty(propertyName);

            if (prop != null)
            {
                var toUpdate = DomainObject.SingleOrDefault(r => prop.GetValue(DomainObject) == value);
                if (toUpdate != null)
                {
                    int id = toUpdate.Id;
                    obj.Id = id;
                    toUpdate = obj;


                    State.Changed.Add(id);
                    State.Added.Remove(id);
                }
            }
        }

        // var type = typeof(U).GetGenericTypeDefinition().GenericTypeArguments[0];
        public void Save()
        {
            // UploadContainer<List<IdWrapper<T>>, IdWrapper<T>, T>(DomainObject, State);
            //UploadContainer<List<IdWrapper<T>>, IdWrapper<T>, T>(DomainObject, State);
            UploadContainer();
            //var hs = new HashSet<IdWrapper<T>>();
            //UploadContainer<List<IdWrapper<T>>, IdWrapper<T>, T>(hs.ToList(), State);
            
            //FIXME
            Console.WriteLine("State was cleared");
            State.Removed.Clear();
            State.Changed.Clear();
            State.Added.Clear();
        }

        public void Delete(int id)  
        {
            var tmp = (T) DomainObject.FirstOrDefault(x => x.Id == id);
            if (tmp != null)
            {
                DomainObject.RemoveAt(tmp.Id); // FIXME
                State.Removed.Add(id);
                State.Added.Remove(tmp.Id);
                State.Changed.Remove(tmp.Id);
            }
        }

        public void DeleteIndex(int index)
        {
            if (index > 0 || index < DomainObject.Count)
            {
                var tmp = DomainObject[index];
                DomainObject.RemoveAt(index);
                State.Removed.Add(tmp.Id);
                State.Added.Remove(tmp.Id);
                State.Changed.Remove(tmp.Id);
            }
        }

        public void Delete(string propertyName, object value)
        {
            PropertyInfo prop = typeof(T).GetProperty(propertyName);

            if (prop != null)
            {
                var obj = DomainObject.SingleOrDefault(r => prop.GetValue(DomainObject) == value);
                if (obj != null)
                {
                    DomainObject.Remove(obj);
                    State.Removed.Add(obj.Id);
                    State.Added.Remove(obj.Id);
                    State.Changed.Remove(obj.Id);
                }
            }
        }

        public void Delete(Expression<Func<T, bool>> expression)
        {
            List<int> to_remove = DomainObject.Where(expression.Compile().Invoke)
                .Select(x => x.Id).ToList();

            State.Removed.UnionWith(to_remove);
            State.Added.ExceptWith(to_remove);
            State.Changed.ExceptWith(to_remove);

            DomainObject.RemoveAll(expression.Compile().Invoke);
        }

        // FIXME doesnt seems right
        public void InsertNew(params object[] args)
        {
            var obj = (T)Activator.CreateInstance(typeof(T), args);
            if (obj != null)
            {
                Insert(obj);
            }
        }

        public void Insert(T tmp)
        {
            //connection
            MaxIdx += 1;
            tmp.Id = MaxIdx;
            //tmp.Id = GetMaxId<T>();
           

            DomainObject.Add(tmp);
            State.Added.Add(tmp.Id);
            State.Removed.Remove(tmp.Id);
            State.Changed.Remove(tmp.Id);
        }

        public DataMapper<T> Fetch(int _, SqlConnector sql) { throw new NotSupportedException("Wrong fetch in DataMapper"); }

        public override string ToString()
        {
            // FIXME
            return ToString(this.GetType().Name.Split('`')[0], DomainObject.ToString());
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in DomainObject)
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public new object this[string propertyName]
        {
            get
            {
                var lsProps = typeof(List<T>).GetProperties();
                foreach (var lsProp in lsProps)
                {
                    if (lsProp.Name == propertyName)
                        return lsProp.GetValue(DomainObject);
                }

                return base[propertyName];
            }
            set
            {
                base[propertyName] = value;
            }
        }

    //    protected void UploadContainer<Container, WrapType, InnerType>(Container _container, StateOfCollection state)
    //where Container : T, IEnumerable<WrapType>
    //where WrapType : IdWrapper<InnerType>
    //where InnerType : new()
        protected void UploadContainer()
        {
            if (!_unitUploadWork.Run(State))
            {
                Console.WriteLine("Upload didnt happen");
            }
        }
    }
}
