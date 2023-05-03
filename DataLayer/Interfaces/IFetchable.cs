using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public interface IFetchable<T>
    {
        public T Fetch();
        public static T Fetch(int id, SqlConnector sql) => throw new NotImplementedException();

        public void Save();
    }
}
