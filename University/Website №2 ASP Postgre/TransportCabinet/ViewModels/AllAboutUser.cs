using TransportCabinet.Models;

namespace TransportCabinet.ViewModels
{
    public class AllAboutUser
	{
		UserAccount User { get; set; } = null;

		TransportCard Card { get; set; } = null;

		string Photo = null;

		public AllAboutUser(UserAccount User, TransportCard Card, string Photo = null) 
		{
			this.Card = Card;
			this.User = User;
			this.Photo = Photo;
		}

		public AllAboutUser()
		{ }

		public UserAccount GetUser() => User;
		public TransportCard GetCard() => Card;
		public string GetPhoto() => Photo;

		public void SetCard(TransportCard card)
		{
			this.Card = card;
		}
		public void SetUser(UserAccount user)
		{
			this.User = user;
		}
	}
}
