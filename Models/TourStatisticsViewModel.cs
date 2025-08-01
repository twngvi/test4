namespace test4.Models
{
    public class TourStatisticsViewModel
    {
        public int TourID { get; set; }
        public string TourName { get; set; } = null!;
        public int NumberOfDestinations { get; set; }
        public int TotalTourDays { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
