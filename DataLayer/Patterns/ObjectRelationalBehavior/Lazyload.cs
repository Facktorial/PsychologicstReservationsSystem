using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Lazyload<T> where T : IFetchable<T>, IHaveIndexer // : PatternPrototype<T> where T : new()
    {
        private readonly Func<T> _fetchFunc;
        private T? _domainObject;

        public Lazyload(params object[] args)
        {
            var method = typeof(T)
                .GetMethods()
                .Where(x => x.Name == "Fetch")
                .First(x => x.GetParameters().Length == args.Length || (x.GetParameters().Length == 0 && args.Length == 1));

            _fetchFunc = () =>
            {
                var parLen = method.GetParameters().Length;
                if (!(parLen == args.Length || (parLen == 0 || args.Length == 1))) { throw new ArgumentException($"Expected {parLen} parameters, but got {args.Length}."); }

                if (args.Length == 1 && parLen == 0)
                {
                    _domainObject = (T) Activator.CreateInstance(typeof(T), args);
                    _domainObject.Fetch();
                    return _domainObject;
                }

                _domainObject = (T)method.Invoke(Activator.CreateInstance(typeof(T)), args);
                return _domainObject;
            };
        }

        private bool LazyLoadThing()
        {
            if (_domainObject == null)
            {
                _domainObject = _fetchFunc();
                return true;
            }
            return false;
        }

        public void Fetch()
        {
            _domainObject.Fetch();
        }

        public void Save()
        {
            _domainObject.Save();
        }

        //public T? DomainObject
        //{
        //    get
        //    {
        //        LazyLoadThing();
        //        return _domainObject;
        //    }
        //    set { _domainObject = value; }
        //}

        public object? this[string propertyName]
        {
            get
            {
                LazyLoadThing();
                Console.WriteLine(typeof(T).Name);

                // FIXME
                //if (_domainObject is DataMapper<IPerson>)
                //{
                //    Console.WriteLine("DataMapper...");
                //}
                //if (_domainObject is PatternPrototype<List<
                //pper<IPerson>>>)
                //{
                //    Console.WriteLine("Pattern");
                //}
                return _domainObject[propertyName];
            }
            set
            {
                LazyLoadThing();
                _domainObject[propertyName] = value;
            }
        }

        public Func<object[], object> this[Method methodName]
        {
            get
            {
                LazyLoadThing();
                return _domainObject[methodName];
            }
        }

        public override string ToString()
        {
            return "Lazy " + _domainObject?.ToString();
        }

        public T? GetObject()
        {
            LazyLoadThing();
            return _domainObject;
        }
    }
}
