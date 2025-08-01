using System;
using System.Collections.Generic;

namespace test4.Data;

public partial class Tour
{
    public int TourId { get; set; }

    public string TourName { get; set; } = null!;

    public int? Duration { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<Destination> Destinations { get; set; } = new List<Destination>();
}
