using System;
using System.Collections.Generic;

namespace test4.Data;

public partial class Destination
{
    public int DestinationId { get; set; }

    public string DestinationName { get; set; } = null!;

    public string? City { get; set; }

    public string? PhotoPath { get; set; }

    public int? TourId { get; set; }

    public virtual Tour? Tour { get; set; }
}
