using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public record ForeignPair<T>(T obj, int id);
    public class ForeignKeyMapping<Parent> ///, ForeignTypes>
    {
        public int Id { get; set; }
        public Parent DomainObject { get; set; }


        private Dictionary<Type, (object obj, int id)> _relatedForeignObjects;

        public ForeignKeyMapping(Parent _domainObj, int id, List<(Type, object, int)> objects)
        {
            DomainObject = _domainObj;
            Id = id;

            _relatedForeignObjects = new Dictionary<Type, (object obj, int id)>();

            foreach ((Type _type, object _obj, int _id) in objects)
            {
                _relatedForeignObjects.Add(_type, (_obj, _id));
            }
        }

        public (Type type, (object obj, int id)) FindObject(Type _type)
        {
            return (_type, _relatedForeignObjects[_type]);
        }

        public override string ToString()
        {
            return $"{DomainObject.ToString()}:\n{ObjectsSerializer()}";
        }

        private string ObjectsSerializer()
        {
            StringWriter stringWriter = new StringWriter();

            foreach ((Type t, (object obj, int id)) in _relatedForeignObjects)
            {
                var objCasted = Convert.ChangeType(obj, t);
                stringWriter.Write("\t" + t.Name + ": " + " Id: " + id + " obj: " + (objCasted?.ToString() ?? "NULL") + ",\n");
            }

            return stringWriter.ToString();
        }
    }
}
