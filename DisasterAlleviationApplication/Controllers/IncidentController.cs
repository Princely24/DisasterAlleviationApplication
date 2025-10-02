using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DisasterAlleviationApplication.Models;

namespace DisasterAlleviationApplication.Controllers
{
    [Authorize]
    public class IncidentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public IncidentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: Incident
        public async Task<IActionResult> Index()
        {
            ViewBag.CurrentUserId = _userManager.GetUserId(User);
            var incidents = await _context.DisasterIncidents
                .Include(d => d.User)
                .OrderByDescending(d => d.ReportedDate)
                .ToListAsync();
            return View(incidents);
        }

        // GET: Incident/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incident = await _context.DisasterIncidents
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (incident == null)
            {
                return NotFound();
            }

            return View(incident);
        }

        // GET: Incident/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Incident/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Type,Severity,Location,City,State,PostalCode,Latitude,Longitude,IncidentDate,AdditionalNotes,EstimatedAffectedPeople,AttachmentFile")] DisasterIncident incident)
        {
            // Assign server-controlled fields before validation
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            incident.UserId = userId;
            incident.User = user;
            incident.ReportedDate = DateTime.UtcNow;
            incident.Status = IncidentStatus.Reported;
            
            // Remove validation errors for properties not in the form
            ModelState.Remove(nameof(DisasterIncident.UserId));
            ModelState.Remove(nameof(DisasterIncident.User));
            ModelState.Remove(nameof(DisasterIncident.ReportedDate));
            ModelState.Remove(nameof(DisasterIncident.Status));
            ModelState.Remove(nameof(DisasterIncident.AttachmentFile));

            // Handle file upload
            if (incident.AttachmentFile != null && incident.AttachmentFile.Length > 0)
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
                var fileExtension = Path.GetExtension(incident.AttachmentFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("AttachmentFile", "Only image files (JPG, PNG, GIF) and PDF files are allowed.");
                }
                
                // Validate file size (max 10MB)
                if (incident.AttachmentFile.Length > 10 * 1024 * 1024)
                {
                    ModelState.AddModelError("AttachmentFile", "File size cannot exceed 10MB.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Save file if uploaded
                    if (incident.AttachmentFile != null && incident.AttachmentFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "incidents");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(incident.AttachmentFile.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await incident.AttachmentFile.CopyToAsync(fileStream);
                        }

                        incident.AttachmentFileName = incident.AttachmentFile.FileName;
                        incident.AttachmentContentType = incident.AttachmentFile.ContentType;
                        incident.AttachmentFileSize = incident.AttachmentFile.Length;
                        incident.AttachmentFilePath = $"/uploads/incidents/{uniqueFileName}";
                        incident.AttachmentUploadedDate = DateTime.UtcNow;
                    }

                    _context.Add(incident);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Disaster incident reported successfully!";
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
            return View(incident);
        }

        // GET: Incident/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incident = await _context.DisasterIncidents.FindAsync(id);
            if (incident == null)
            {
                return NotFound();
            }

            // Only allow the user who created the incident to edit it
            if (incident.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            return View(incident);
        }

        // POST: Incident/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Type,Severity,Location,City,State,PostalCode,Latitude,Longitude,IncidentDate,Status,AdditionalNotes,EstimatedAffectedPeople")] DisasterIncident incident)
        {
            if (id != incident.Id)
            {
                return NotFound();
            }

            var existingIncident = await _context.DisasterIncidents.FindAsync(id);
            if (existingIncident == null)
            {
                return NotFound();
            }

            // Only allow the user who created the incident to edit it
            if (existingIncident.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingIncident.Title = incident.Title;
                    existingIncident.Description = incident.Description;
                    existingIncident.Type = incident.Type;
                    existingIncident.Severity = incident.Severity;
                    existingIncident.Location = incident.Location;
                    existingIncident.City = incident.City;
                    existingIncident.State = incident.State;
                    existingIncident.PostalCode = incident.PostalCode;
                    existingIncident.Latitude = incident.Latitude;
                    existingIncident.Longitude = incident.Longitude;
                    existingIncident.IncidentDate = incident.IncidentDate;
                    existingIncident.Status = incident.Status;
                    existingIncident.AdditionalNotes = incident.AdditionalNotes;
                    existingIncident.EstimatedAffectedPeople = incident.EstimatedAffectedPeople;

                    _context.Update(existingIncident);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Incident updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisasterIncidentExists(incident.Id))
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
            return View(incident);
        }

        // GET: Incident/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incident = await _context.DisasterIncidents
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (incident == null)
            {
                return NotFound();
            }

            // Only allow the user who created the incident to delete it
            if (incident.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            return View(incident);
        }

        // POST: Incident/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var incident = await _context.DisasterIncidents.FindAsync(id);
            if (incident != null)
            {
                // Only allow the user who created the incident to delete it
                if (incident.UserId == _userManager.GetUserId(User))
                {
                    _context.DisasterIncidents.Remove(incident);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Incident deleted successfully!";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DisasterIncidentExists(int id)
        {
            return _context.DisasterIncidents.Any(e => e.Id == id);
        }
    }
}
