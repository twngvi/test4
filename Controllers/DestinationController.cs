using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using test4.Data;
using test4.ViewModels;

namespace test4.Controllers
{
    public class DestinationController : Controller
    {
        private readonly Test4Context _context;

        public DestinationController(Test4Context context)
        {
            _context = context;
        }

        // GET: Destination
        public async Task<IActionResult> Index()
        {
            var destinations = await _context.Destinations
                .Include(d => d.Tour)
                .ToListAsync();
            return View(destinations);
        }

        // GET: Destination/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var destination = await _context.Destinations
                .Include(d => d.Tour)
                .FirstOrDefaultAsync(m => m.DestinationId == id);

            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }

        // GET: Destination/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new DestinationViewModel
            {
                Tours = await GetToursSelectList()
            };
            return View(viewModel);
        }

        // POST: Destination/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DestinationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(viewModel.Destination);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            viewModel.Tours = await GetToursSelectList();
            return View(viewModel);
        }

        // GET: Destination/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var destination = await _context.Destinations.FindAsync(id);
            if (destination == null)
            {
                return NotFound();
            }

            var viewModel = new DestinationViewModel
            {
                Destination = destination,
                Tours = await GetToursSelectList()
            };
            return View(viewModel);
        }

        // POST: Destination/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DestinationViewModel viewModel)
        {
            if (id != viewModel.Destination.DestinationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel.Destination);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DestinationExists(viewModel.Destination.DestinationId))
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
            viewModel.Tours = await GetToursSelectList();
            return View(viewModel);
        }

        // GET: Destination/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var destination = await _context.Destinations
                .Include(d => d.Tour)
                .FirstOrDefaultAsync(m => m.DestinationId == id);

            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }

        // POST: Destination/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var destination = await _context.Destinations.FindAsync(id);
            if (destination != null)
            {
                _context.Destinations.Remove(destination);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DestinationExists(int id)
        {
            return _context.Destinations.Any(e => e.DestinationId == id);
        }

        private async Task<List<SelectListItem>> GetToursSelectList()
        {
            var tours = await _context.Tours.ToListAsync();
            var selectList = tours.Select(t => new SelectListItem
            {
                Value = t.TourId.ToString(),
                Text = t.TourName
            }).ToList();
            
            selectList.Insert(0, new SelectListItem { Value = "", Text = "-- Select Tour --" });
            return selectList;
        }
    }
}
