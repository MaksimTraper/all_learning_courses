using TransportCabinet.Models;

namespace TransportCabinet.ViewModels
{
	public class VehiclesView
	{
		public List<AllAboutCar> cars { get; set; }
		public AllAboutUser user { get; set; }

		public VehiclesView(List<AllAboutCar> cars, AllAboutUser user)
		{
			this.cars = cars;
			this.user = user;
		}

		public void SetCar(List<AllAboutCar> cars)
		{
			this.cars = cars;
		}
		public void SetUser(AllAboutUser user) 
		{
			this.user = user;
		}
		public List<AllAboutCar> GetCars() => this.cars;
		public AllAboutUser GetUser() => this.user;
	}
}
