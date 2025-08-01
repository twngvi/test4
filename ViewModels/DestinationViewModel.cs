using Microsoft.AspNetCore.Mvc.Rendering;
using test4.Data;

namespace test4.ViewModels
{
    public class DestinationViewModel
    {
        public Destination Destination { get; set; } = new Destination();
        public List<SelectListItem> Tours { get; set; } = new List<SelectListItem>();
    }
}
