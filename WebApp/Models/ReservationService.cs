using DataLayer;
using DataLayer.Models;

namespace WebApp.Models
{
    public class ReservationService
    {
        // TODO 
        private SqlConnector Sql = new SqlConnector(@"Data Source=..\..\..\..\..\Data\psycho.db");
        public DataMapper<Reservation> Mapper;
        public bool IsFetched = false;

        public List<Reservation> Reservations() { return Mapper.DomainObject; }

        public void Fetch()
        {
            Mapper = new DataMapper<Reservation>(Sql);
            Mapper.Fetch();
            IsFetched = true;
        }

        public Reservation? Get(int id)
        {
            return Reservations().FirstOrDefault(x => x.Id == id);
        }

        public Reservation? Get(Reservation p)
        {
            return Reservations().FirstOrDefault(x => x.Id == p.Id);
        }

        public void Upload(Reservation r)
        {
            Mapper.Insert(r);
            Mapper.Save();
            Mapper.Fetch();
        }

        public ReservationService(SqlConnector sql)
        {
            Sql = sql;
            Mapper = new DataMapper<Reservation>(Sql);
        }
    }
}
