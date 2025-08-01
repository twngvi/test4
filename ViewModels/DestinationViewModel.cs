using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using test4.Data;

namespace test4.ViewModels
{
    public class DestinationViewModel
    {
        public Destination Destination { get; set; } = new Destination();
        public List<SelectListItem> Tours { get; set; } = new List<SelectListItem>();
        
        [Display(Name = "Chọn ảnh")]
        public IFormFile? PhotoFile { get; set; }
    }
}
