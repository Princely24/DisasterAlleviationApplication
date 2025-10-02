using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DisasterAlleviationApplication.Models;

namespace DisasterAlleviationApplication.Controllers
{
    [Authorize]
    public class VolunteerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VolunteerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Volunteer
        public async Task<IActionResult> Index()
        {
            ViewBag.CurrentUserId = _userManager.GetUserId(User);
            var volunteers = await _context.Volunteers
                .Include(v => v.User)
                .OrderByDescending(v => v.RegistrationDate)
                .ToListAsync();
            return View(volunteers);
        }

        // GET: Volunteer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteers
                .Include(v => v.User)
                .Include(v => v.Assignments)
                .ThenInclude(a => a.Task)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // GET: Volunteer/Create
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            var existingVolunteer = await _context.Volunteers.FirstOrDefaultAsync(v => v.UserId == userId);
            
            if (existingVolunteer != null)
            {
                TempData["InfoMessage"] = "You already have a volunteer profile. You can update it instead.";
                return RedirectToAction(nameof(Edit), new { id = existingVolunteer.Id });
            }

            return View();
        }

        // POST: Volunteer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PhoneNumber,Address,City,State,PostalCode,DateOfBirth,EmergencyContactName,EmergencyContactPhone,Skills,Interests,Availability,Notes")] Volunteer volunteer)
        {
            // Assign server-controlled fields before validation
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            volunteer.UserId = userId;
            volunteer.User = user;
            volunteer.RegistrationDate = DateTime.UtcNow;
            volunteer.Status = VolunteerStatus.Pending;
            volunteer.Email = user.Email ?? volunteer.Email;
            
            // Remove validation errors for properties not in the form
            ModelState.Remove(nameof(Volunteer.UserId));
            ModelState.Remove(nameof(Volunteer.User));
            ModelState.Remove(nameof(Volunteer.RegistrationDate));
            ModelState.Remove(nameof(Volunteer.Status));
            ModelState.Remove(nameof(Volunteer.Assignments));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(volunteer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Volunteer registration submitted successfully! We will review your application and contact you soon.";
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
                        // These will appear in the validation summary on the form
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }
            }
            return View(volunteer);
        }

        // GET: Volunteer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            // Only allow the volunteer to edit their own profile
            if (volunteer.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            return View(volunteer);
        }

        // POST: Volunteer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,Address,City,State,PostalCode,DateOfBirth,EmergencyContactName,EmergencyContactPhone,Skills,Interests,Availability,Notes")] Volunteer volunteer)
        {
            if (id != volunteer.Id)
            {
                return NotFound();
            }

            var existingVolunteer = await _context.Volunteers.FindAsync(id);
            if (existingVolunteer == null)
            {
                return NotFound();
            }

            // Only allow the volunteer to edit their own profile
            if (existingVolunteer.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingVolunteer.FirstName = volunteer.FirstName;
                    existingVolunteer.LastName = volunteer.LastName;
                    existingVolunteer.Email = volunteer.Email;
                    existingVolunteer.PhoneNumber = volunteer.PhoneNumber;
                    existingVolunteer.Address = volunteer.Address;
                    existingVolunteer.City = volunteer.City;
                    existingVolunteer.State = volunteer.State;
                    existingVolunteer.PostalCode = volunteer.PostalCode;
                    existingVolunteer.DateOfBirth = volunteer.DateOfBirth;
                    existingVolunteer.EmergencyContactName = volunteer.EmergencyContactName;
                    existingVolunteer.EmergencyContactPhone = volunteer.EmergencyContactPhone;
                    existingVolunteer.Skills = volunteer.Skills;
                    existingVolunteer.Interests = volunteer.Interests;
                    existingVolunteer.Availability = volunteer.Availability;
                    existingVolunteer.Notes = volunteer.Notes;

                    _context.Update(existingVolunteer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Volunteer profile updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolunteerExists(volunteer.Id))
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
            return View(volunteer);
        }

        // GET: Volunteer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteers
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (volunteer == null)
            {
                return NotFound();
            }

            // Only allow the volunteer to delete their own profile
            if (volunteer.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            return View(volunteer);
        }

        // POST: Volunteer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer != null)
            {
                // Only allow the volunteer to delete their own profile
                if (volunteer.UserId == _userManager.GetUserId(User))
                {
                    _context.Volunteers.Remove(volunteer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Volunteer profile deleted successfully!";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Volunteer/MyProfile
        public async Task<IActionResult> MyProfile()
        {
            var userId = _userManager.GetUserId(User);
            var volunteer = await _context.Volunteers
                .Include(v => v.Assignments)
                .ThenInclude(a => a.Task)
                .FirstOrDefaultAsync(v => v.UserId == userId);

            if (volunteer == null)
            {
                TempData["InfoMessage"] = "You don't have a volunteer profile yet. Would you like to create one?";
                return RedirectToAction(nameof(Create));
            }

            return View(volunteer);
        }

        private bool VolunteerExists(int id)
        {
            return _context.Volunteers.Any(e => e.Id == id);
        }
    }
}
