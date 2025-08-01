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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DestinationController(Test4Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Destination
        public async Task<IActionResult> Index(string searchTerm)
        {
            var destinations = await GetDestinationsAsync(searchTerm);
            
            var viewModel = new DestinationSearchViewModel
            {
                SearchTerm = searchTerm ?? string.Empty,
                Destinations = destinations
            };
            
            return View(viewModel);
        }

        // AJAX search endpoint
        [HttpGet]
        public async Task<IActionResult> SearchDestinations(string searchTerm)
        {
            var destinations = await GetDestinationsAsync(searchTerm);
            return PartialView("_DestinationResults", destinations);
        }

        private async Task<List<Destination>> GetDestinationsAsync(string searchTerm)
        {
            var query = _context.Destinations.Include(d => d.Tour).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(d => 
                    d.DestinationName.Contains(searchTerm) || 
                    (d.City != null && d.City.Contains(searchTerm)));
            }

            return await query.ToListAsync();
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
            // Kiểm tra và validate file ảnh nếu được upload
            if (viewModel.PhotoFile != null)
            {
                var validationResult = ValidatePhotoFile(viewModel.PhotoFile);
                if (!validationResult.IsValid)
                {
                    ModelState.AddModelError("PhotoFile", validationResult.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh
                if (viewModel.PhotoFile != null)
                {
                    var photoPath = await SavePhotoAsync(viewModel.PhotoFile);
                    viewModel.Destination.PhotoPath = photoPath;
                }

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

            // Kiểm tra và validate file ảnh nếu được upload
            if (viewModel.PhotoFile != null)
            {
                var validationResult = ValidatePhotoFile(viewModel.PhotoFile);
                if (!validationResult.IsValid)
                {
                    ModelState.AddModelError("PhotoFile", validationResult.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý upload ảnh mới
                    if (viewModel.PhotoFile != null)
                    {
                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(viewModel.Destination.PhotoPath))
                        {
                            DeletePhoto(viewModel.Destination.PhotoPath);
                        }
                        
                        var photoPath = await SavePhotoAsync(viewModel.PhotoFile);
                        viewModel.Destination.PhotoPath = photoPath;
                    }

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
                // Xóa ảnh liên quan nếu có
                if (!string.IsNullOrEmpty(destination.PhotoPath))
                {
                    DeletePhoto(destination.PhotoPath);
                }
                
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
            
            selectList.Insert(0, new SelectListItem { Value = "", Text = "-- Chọn Tour --" });
            return selectList;
        }

        #region Photo Upload Methods

        private (bool IsValid, string ErrorMessage) ValidatePhotoFile(IFormFile file)
        {
            // Kiểm tra kích thước file (tối đa 2MB)
            if (file.Length > 2 * 1024 * 1024)
            {
                return (false, "Kích thước file phải nhỏ hơn 2MB");
            }

            // Kiểm tra loại file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return (false, "Chỉ chấp nhận file JPG và PNG");
            }

            return (true, string.Empty);
        }

        private async Task<string> SavePhotoAsync(IFormFile file)
        {
            // Tạo tên file duy nhất sử dụng GUID
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "destinations");
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Đảm bảo thư mục tồn tại
            Directory.CreateDirectory(uploadsFolder);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/images/destinations/{fileName}";
        }

        private void DeletePhoto(string photoPath)
        {
            if (string.IsNullOrEmpty(photoPath)) return;

            var fileName = Path.GetFileName(photoPath);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "destinations", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        #endregion
    }
}
