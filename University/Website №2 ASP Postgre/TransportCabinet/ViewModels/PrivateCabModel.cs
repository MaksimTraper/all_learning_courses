using Microsoft.AspNetCore.Mvc;

namespace TransportCabinet.ViewModels
{
    public class PrivateCabModel
    {
        AllAboutUser user { get; set; } = null;

        string message { get; set; } = null;

        public PrivateCabModel(AllAboutUser user, string message = null)
        {
            this.user = user;
            this.message = message;
        }

        public string getMessage() => message;
        public AllAboutUser getUser() => user;
    }
}
