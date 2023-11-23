namespace TransportCabinet.ViewModels
{
    public class SqlQueryModel
    {
        public string query { get; set; } = null;

        public AllAboutUser user { get; set; } = null;

        public List<string> results { get; set; } = null;
        public int CountColumns { get; set; } = 0;

        public List<string> GetResults() { return results; }

        public string message { get; set; } = null;

        public SqlQueryModel(string query = null, AllAboutUser user = null, List<string> results = null, int CountColumns = 0, string message = null)
        {
            this.query = query;
            this.user = user;
            this.results = results;
            this.CountColumns = CountColumns;
            this.message = message;
        }
        public SqlQueryModel() { }
    }
}
