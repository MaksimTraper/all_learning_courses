namespace TransportCabinet.Models
{
    public class NamesVariables
    {
        public List<string> valuesRoute { get; set; } = new List<string> { "pk_id_route", "start_point", "end_point" };
        public List<string> valuesDriver { get; set; } = new List<string> { "pk_id_driver", "name", "surname", "patronymic" };
        public List<string> valuesTransport { get; set; } = new List<string> { "pk_car_num", "fk_id_vehicle" };
        public List<string> valuesTransportCard { get; set; } = new List<string> { "pk_id_card", "fk_id_owner", "balance", "data_issue", "num_days" };
        public List<string> valuesTransportVehicle { get; set; } = new List<string> { "pk_id_vehicle", "brand", "model" };
        public List<string> valuesTrip { get; set; } = new List<string> { "pk_num_trip", "fk_id_tr", "fk_id_card", "fk_id_driver", "time_pay", "num_route" };
        public List<string> valuesUser { get; set; } = new List<string> { "passport_num", "name", "surname", "patronymic", "pk_login", "password", "email", "role", "birthday" };
        public List<string> valuesPurchase { get; set; } = new List<string> { "pk_num_purchase", "fk_id_card", "name_purchase", "price", "amount" };
    }
}
