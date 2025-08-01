using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using test4.Data;
using test4.Models;

namespace test4.Controllers
{
    public class TourStatisticsController : Controller
    {
        private readonly Test4Context _context;

        public TourStatisticsController(Test4Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Statistics()
        {
            var statistics = await _context.Destinations
                .Where(d => d.TourId != null)
                .GroupBy(d => new { d.TourId, d.Tour.TourName })
                .Select(g => new TourStatisticsViewModel
                {
                    TourID = g.Key.TourId ?? 0,
                    TourName = g.Key.TourName,
                    NumberOfDestinations = g.Count(),
                    TotalTourDays = g.Sum(x => x.Tour != null && x.Tour.Duration.HasValue ? x.Tour.Duration.Value : 0),
                    TotalPrice = g.Sum(x => x.Tour != null && x.Tour.Price.HasValue ? x.Tour.Price.Value : 0)
                })
                .ToListAsync();

            return View("~/Views/Statistics/Statistics.cshtml", statistics);
        }
    }
}

