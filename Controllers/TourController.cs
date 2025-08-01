using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test4.Data;
using test4.Models;

namespace test4.Controllers
{
    public class TourController : Controller
    {
        private readonly Test4Context _context;

        public TourController(Test4Context context)
        {
            _context = context;
        }

        // GET: Tour
        public async Task<IActionResult> Index()
        {
            var tours = await _context.Tours
                .Include(t => t.Destinations)
                .Select(t => new TourViewModel
                {
                    TourId = t.TourId,
                    TourName = t.TourName,
                    Duration = t.Duration,
                    Price = t.Price,
                    DestinationCount = t.Destinations.Count(),
                    Destinations = t.Destinations.ToList()
                })
                .ToListAsync();

            return View(tours);
        }

        // GET: Tour/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .Include(t => t.Destinations)
                .FirstOrDefaultAsync(m => m.TourId == id);

            if (tour == null)
            {
                return NotFound();
            }

            var viewModel = new TourViewModel
            {
                TourId = tour.TourId,
                TourName = tour.TourName,
                Duration = tour.Duration,
                Price = tour.Price,
                DestinationCount = tour.Destinations.Count(),
                Destinations = tour.Destinations.ToList()
            };

            return View(viewModel);
        }

        // GET: Tour/Create
        public IActionResult Create()
        {
            return View(new TourCreateEditViewModel());
        }

        // POST: Tour/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TourCreateEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var tour = new Tour
                {
                    TourName = viewModel.TourName,
                    Duration = viewModel.Duration,
                    Price = viewModel.Price
                };

                _context.Add(tour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Tour/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                return NotFound();
            }

            var viewModel = new TourCreateEditViewModel
            {
                TourId = tour.TourId,
                TourName = tour.TourName,
                Duration = tour.Duration,
                Price = tour.Price
            };

            return View(viewModel);
        }

        // POST: Tour/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TourCreateEditViewModel viewModel)
        {
            if (id != viewModel.TourId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var tour = await _context.Tours.FindAsync(id);
                    if (tour == null)
                    {
                        return NotFound();
                    }

                    tour.TourName = viewModel.TourName;
                    tour.Duration = viewModel.Duration;
                    tour.Price = viewModel.Price;

                    _context.Update(tour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TourExists(viewModel.TourId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Tour/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .Include(t => t.Destinations)
                .FirstOrDefaultAsync(m => m.TourId == id);

            if (tour == null)
            {
                return NotFound();
            }

            var viewModel = new TourViewModel
            {
                TourId = tour.TourId,
                TourName = tour.TourName,
                Duration = tour.Duration,
                Price = tour.Price,
                DestinationCount = tour.Destinations.Count(),
                Destinations = tour.Destinations.ToList()
            };

            return View(viewModel);
        }

        // POST: Tour/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour != null)
            {
                _context.Tours.Remove(tour);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TourExists(int id)
        {
            return _context.Tours.Any(e => e.TourId == id);
        }
    }
}
