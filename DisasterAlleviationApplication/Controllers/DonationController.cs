using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DisasterAlleviationApplication.Models;

namespace DisasterAlleviationApplication.Controllers
{
    [Authorize]
    public class DonationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DonationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Donation
        public async Task<IActionResult> Index()
        {
            ViewBag.CurrentUserId = _userManager.GetUserId(User);
            var donations = await _context.Donations
                .Include(d => d.User)
                .Include(d => d.Category)
                .OrderByDescending(d => d.DonationDate)
                .ToListAsync();
            return View(donations);
        }

        // GET: Donation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donation = await _context.Donations
                .Include(d => d.User)
                .Include(d => d.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (donation == null)
            {
                return NotFound();
            }

            return View(donation);
        }

        // GET: Donation/Create
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(await _context.DonationCategories.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: Donation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemName,Description,Quantity,Unit,EstimatedValue,Type,CategoryId,PickupLocation,PickupDate,Notes,IsUrgent,ExpiryDate")] Donation donation)
        {
            // Assign server-controlled fields before validation
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var category = await _context.DonationCategories.FindAsync(donation.CategoryId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            donation.UserId = userId;
            donation.User = user;
            donation.Category = category;
            donation.DonationDate = DateTime.UtcNow;
            donation.Status = DonationStatus.Pending;
            
            // Remove validation errors for properties not in the form
            ModelState.Remove(nameof(Donation.UserId));
            ModelState.Remove(nameof(Donation.User));
            ModelState.Remove(nameof(Donation.Category));
            ModelState.Remove(nameof(Donation.DonationDate));
            ModelState.Remove(nameof(Donation.Status));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(donation);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Donation submitted successfully! Thank you for your contribution.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error saving to database: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        ModelState.AddModelError("", $"Inner exception: {ex.InnerException.Message}");
                    }
                }
            }
            else
            {
                // Log validation errors to help debug
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }
            }
            ViewData["CategoryId"] = new SelectList(await _context.DonationCategories.Where(c => c.IsActive).ToListAsync(), "Id", "Name", donation.CategoryId);
            return View(donation);
        }

        // GET: Donation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
            {
                return NotFound();
            }

            // Only allow the user who made the donation to edit it
            if (donation.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            // Don't allow editing if donation has been processed
            if (donation.Status != DonationStatus.Pending)
            {
                TempData["ErrorMessage"] = "Cannot edit donation that has already been processed.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(await _context.DonationCategories.Where(c => c.IsActive).ToListAsync(), "Id", "Name", donation.CategoryId);
            return View(donation);
        }

        // POST: Donation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemName,Description,Quantity,Unit,EstimatedValue,Type,CategoryId,PickupLocation,PickupDate,Notes,IsUrgent,ExpiryDate")] Donation donation)
        {
            if (id != donation.Id)
            {
                return NotFound();
            }

            var existingDonation = await _context.Donations.FindAsync(id);
            if (existingDonation == null)
            {
                return NotFound();
            }

            // Only allow the user who made the donation to edit it
            if (existingDonation.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            // Don't allow editing if donation has been processed
            if (existingDonation.Status != DonationStatus.Pending)
            {
                TempData["ErrorMessage"] = "Cannot edit donation that has already been processed.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingDonation.ItemName = donation.ItemName;
                    existingDonation.Description = donation.Description;
                    existingDonation.Quantity = donation.Quantity;
                    existingDonation.Unit = donation.Unit;
                    existingDonation.EstimatedValue = donation.EstimatedValue;
                    existingDonation.Type = donation.Type;
                    existingDonation.CategoryId = donation.CategoryId;
                    existingDonation.PickupLocation = donation.PickupLocation;
                    existingDonation.PickupDate = donation.PickupDate;
                    existingDonation.Notes = donation.Notes;
                    existingDonation.IsUrgent = donation.IsUrgent;
                    existingDonation.ExpiryDate = donation.ExpiryDate;

                    _context.Update(existingDonation);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Donation updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonationExists(donation.Id))
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
            ViewData["CategoryId"] = new SelectList(await _context.DonationCategories.Where(c => c.IsActive).ToListAsync(), "Id", "Name", donation.CategoryId);
            return View(donation);
        }

        // GET: Donation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donation = await _context.Donations
                .Include(d => d.User)
                .Include(d => d.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (donation == null)
            {
                return NotFound();
            }

            // Only allow the user who made the donation to delete it
            if (donation.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            // Don't allow deleting if donation has been processed
            if (donation.Status != DonationStatus.Pending)
            {
                TempData["ErrorMessage"] = "Cannot delete donation that has already been processed.";
                return RedirectToAction(nameof(Index));
            }

            return View(donation);
        }

        // POST: Donation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation != null)
            {
                // Only allow the user who made the donation to delete it
                if (donation.UserId == _userManager.GetUserId(User) && donation.Status == DonationStatus.Pending)
                {
                    _context.Donations.Remove(donation);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Donation deleted successfully!";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DonationExists(int id)
        {
            return _context.Donations.Any(e => e.Id == id);
        }
    }
}
