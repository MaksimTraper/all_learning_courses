namespace TransportCabinet.ViewModels
{
    public class BuyCardModel
    {
        public double balance { get; set; }

        public double price { get; set; }
        public int days { get; set; }
        public string message { get; set; } = null;
        public AllAboutUser user { get; set; } = null;
        public BuyCardModel() { }
        public BuyCardModel(string message) { this.message = message; }
    }
}
