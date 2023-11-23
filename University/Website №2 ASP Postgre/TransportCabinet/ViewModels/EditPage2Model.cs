using TransportCabinet.Models;

namespace TransportCabinet.ViewModels
{
    public class EditPage2Model
    {
        public AllAboutUser user { get; set; }
        public string selected { get;set; }
        public int id { get;set; }
        public NamesVariables names { get; set; } = new NamesVariables();

        public UserAccount userAc { get; set; }
        public Driver driver { get; set; }
        public Models.Route route { get; set; }
        public Transport transport { get; set; }
        public TransportCard card { get; set; }
        public TransportVehicle vehicle { get; set; }
        public Trip trip { get; set; }
        public Purchase purchase { get; set; }

        public EditPage2Model(AllAboutUser user, string selected, int id, UserAccount userAc = null, Driver driver = null, Models.Route route = null, 
            Transport transport = null, TransportCard card = null, TransportVehicle vehicle = null, Trip trip = null, Purchase purchase = null)
        {
            this.user = user;
            this.selected = selected;
            this.id = id;
            this.userAc = userAc;
            this.driver = driver;
            this.route = route;
            this.transport = transport;
            this.card = card;
            this.vehicle = vehicle;
            this.trip = trip;
            this.purchase = purchase;
        }

        public EditPage2Model() { }
    }
}
