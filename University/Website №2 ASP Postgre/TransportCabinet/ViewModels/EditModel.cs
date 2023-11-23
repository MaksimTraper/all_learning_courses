using Microsoft.AspNetCore.Mvc.Rendering;
using TransportCabinet.Models;
using Route = TransportCabinet.Models.Route;

namespace TransportCabinet.ViewModels
{
    public class EditModel
    {
        public AllAboutUser user { get; set; }

        public Driver driver { get; set; }
        public Route route { get; set; }
        public Transport transport { get; set; }
        public TransportCard card { get; set; }
        public TransportVehicle vehicle { get; set; }
        public Trip trip { get; set; }
        public Purchase purchase { get; set; }
        public string exception { get; set; }


        public List<UserAccount> users { get; set; }
        public List<Driver> drivers { get; set; }
        public List<Route> routes { get; set; }
        public List<Transport> transports { get; set; }
        public List<TransportCard> transportCards { get; set; }
        public List<TransportVehicle> transportVehicles { get; set; }
        public List<Trip> trips { get; set; }
        public List<Purchase> purchases { get; set; }

        public SelectList classes { get; set; } = new SelectList(new List<string> { "Водители", "Маршруты", "Транспорт", "Транспортные карты", "Модели транспорта", "Поездки", "Пользователи", "Случаи пополнения", });

        public string selected { get; set; } = null;
        public string submitButton { get; set; } = null;

        public NamesVariables names { get; set; } = new NamesVariables ();

        public EditModel previousModel { get; set; } = null;

        public EditModel() { }
        public EditModel(AllAboutUser user, string exception = null) {  this.user = user; this.exception = exception; }
    }
}
