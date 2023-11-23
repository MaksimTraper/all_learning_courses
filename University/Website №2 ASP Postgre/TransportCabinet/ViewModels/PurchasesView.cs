using TransportCabinet.Models;

namespace TransportCabinet.ViewModels
{
	public class PurchasesView
	{
		public List<Purchase> purchases { get; set; }
		public AllAboutUser user { get; set; }

		public PurchasesView(List<Purchase> purchases, AllAboutUser user)
		{
			this.purchases = purchases;
			this.user = user;
		}

		public void SetPurchase(List<Purchase> purchases)
		{
			this.purchases = purchases;
		}
		public void SetUser(AllAboutUser user) 
		{
			this.user = user;
		}
		public List<Purchase> GetPurchases() => this.purchases;
		public AllAboutUser GetUser() => this.user;
	}
}
