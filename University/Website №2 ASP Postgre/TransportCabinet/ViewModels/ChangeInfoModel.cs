namespace TransportCabinet.ViewModels
{
    public class ChangeInfoModel
    {
        public string pk_passport_num { get; set; } = null!;

        public string? name { get; set; } = null;

        public string? surname { get; set; } = null;

        public string? patronymic { get; set; } = null;

        public string? login { get; set; } = null;

        public string? email { get; set; } = null;

        public DateOnly? birthday { get; set; } = null;
    }
}
