using test4.Data;

namespace test4.Models
{
    public class TourViewModel
    {
        public int TourId { get; set; }
        public string TourName { get; set; } = null!;
        public int? Duration { get; set; }
        public decimal? Price { get; set; }
        public int DestinationCount { get; set; }
        public List<Destination> Destinations { get; set; } = new List<Destination>();
    }

    public class TourCreateEditViewModel
    {
        public int TourId { get; set; }
        public string TourName { get; set; } = null!;
        public int? Duration { get; set; }
        public decimal? Price { get; set; }
    }
}
