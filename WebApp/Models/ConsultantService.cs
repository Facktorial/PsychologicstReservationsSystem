using DataLayer;
using DataLayer.Models;

namespace WebApp.Models
{
    public class ConsultantService
    {
        public bool IsFetched = false;
        private SqlConnector Sql;
        //private SqlConnector Sql = new SqlConnector(@"Data Source=..\..\..\..\..\Data\psycho.db");
        public DataMapper<Consultant> Mapper;
        public List<Consultant> Consultants() { return Mapper.DomainObject; }

        public void Fetch()
        {
            Mapper = new DataMapper<Consultant>(Sql);
            Mapper.Fetch();
            IsFetched = true;
        }

        public Consultant? Get(int id)
        {
            return Consultants().FirstOrDefault(x => x.Id == id);
        }

        public Consultant? Get(Consultant p)
        {
            return Consultants().FirstOrDefault(x => x.Id == p.Id);
        }

        public void Upload(Consultant p)
        {
            Mapper.Insert(p);
            Mapper.Save();
            Mapper.Fetch();
        }

        public ConsultantService(SqlConnector sql)
        {
            Sql = sql;
            Mapper = new DataMapper<Consultant>(Sql);
        }
    }
}