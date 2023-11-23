using TransportCabinet.Models;

namespace TransportCabinet.ViewModels
{
    public class HistoryView
    {
        public List<Trip> trips { get; set; }
        public AllAboutUser user { get; set; }
        public List<Driver> drivers { get; set; }

        public HistoryView(List<Trip> trips, AllAboutUser user, List<Driver> drivers)
        {
            this.trips = trips;
            this.user = user;
            this.drivers = drivers;
        }

        public void SetTrips(List<Trip> trips)
        {
            this.trips = trips;
        }
        public void SetUser(AllAboutUser user)
        {
            this.user = user;
        }
        public List<Trip> GetTrips() => this.trips;
        public AllAboutUser GetUser() => this.user;
        public string GetNeedDriver(int ID)
        {
            for (int i = 0; i < this.drivers.Count; i++)
            {
                if (drivers[i].pk_id_driver == ID)
                { return this.drivers[i].surname + " " + this.drivers[i].name + " " + this.drivers[i].patronymic; }
            }
            return null;
        }
    }
}
