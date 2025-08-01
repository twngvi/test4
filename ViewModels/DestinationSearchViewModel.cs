using test4.Data;

namespace test4.ViewModels
{
    public class DestinationSearchViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public IEnumerable<Destination> Destinations { get; set; } = new List<Destination>();
    }
}
