using TransportCabinet.Models;

namespace TransportCabinet.ViewModels
{
    public class AllAboutCar
	{
		public TransportVehicle transportVehicle { get; set; }
		public Transport car { get; set; }

		public AllAboutCar() { }

		public AllAboutCar(TransportVehicle transportVehicle, Transport car) 
		{
			this.transportVehicle = transportVehicle;
			this.car = car;
		}

		public void SetCar(Transport car)
		{
			this.car = car;
		}
		public void SetVehicle(TransportVehicle transportVehicle)
		{
			this.transportVehicle = transportVehicle;
		}

		public Transport GetCar() => car;
		public TransportVehicle GetVehicle() => this.transportVehicle;
	}
}
