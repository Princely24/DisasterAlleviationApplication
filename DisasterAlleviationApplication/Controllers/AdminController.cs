using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DisasterAlleviationApplication.Models;
using System.ComponentModel.DataAnnotations;

namespace DisasterAlleviationApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // GET: Admin/Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Admin/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AdminRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign Admin role
                    await _userManager.AddToRoleAsync(user, "Admin");
                    
                    TempData["SuccessMessage"] = "Admin account created successfully! You can now login.";
                    return RedirectToAction(nameof(Login));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new AdminDashboardViewModel
            {
                PendingVolunteers = await _context.Volunteers
                    .Include(v => v.User)
                    .Where(v => v.Status == VolunteerStatus.Pending)
                    .OrderByDescending(v => v.RegistrationDate)
                    .ToListAsync(),

                TotalVolunteers = await _context.Volunteers.CountAsync(),
                ApprovedVolunteers = await _context.Volunteers.CountAsync(v => v.Status == VolunteerStatus.Approved || v.Status == VolunteerStatus.Active),
                PendingVolunteersCount = await _context.Volunteers.CountAsync(v => v.Status == VolunteerStatus.Pending),

                TotalIncidents = await _context.DisasterIncidents.CountAsync(),
                ActiveIncidents = await _context.DisasterIncidents.CountAsync(i => i.Status == IncidentStatus.InProgress || i.Status == IncidentStatus.UnderReview),

                TotalDonations = await _context.Donations.CountAsync(),
                PendingDonations = await _context.Donations.CountAsync(d => d.Status == DonationStatus.Pending),

                TotalTasks = await _context.VolunteerTasks.CountAsync(),
                OpenTasks = await _context.VolunteerTasks.CountAsync(t => t.Status == Models.TaskStatus.Open)
            };

            return View(viewModel);
        }

        // GET: Admin/Volunteers
        public async Task<IActionResult> Volunteers(string status = "all")
        {
            var query = _context.Volunteers.Include(v => v.User).AsQueryable();

            if (status != "all")
            {
                if (Enum.TryParse<VolunteerStatus>(status, true, out var volunteerStatus))
                {
                    query = query.Where(v => v.Status == volunteerStatus);
                }
            }

            var volunteers = await query.OrderByDescending(v => v.RegistrationDate).ToListAsync();
            ViewBag.CurrentStatus = status;
            return View(volunteers);
        }

        // POST: Admin/ApproveVolunteer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveVolunteer(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            volunteer.Status = VolunteerStatus.Approved;
            volunteer.LastActiveDate = DateTime.UtcNow;
            _context.Update(volunteer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Volunteer {volunteer.FirstName} {volunteer.LastName} has been approved!";
            return RedirectToAction(nameof(Dashboard));
        }

        // POST: Admin/RejectVolunteer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectVolunteer(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            volunteer.Status = VolunteerStatus.Suspended;
            _context.Update(volunteer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Volunteer {volunteer.FirstName} {volunteer.LastName} has been rejected.";
            return RedirectToAction(nameof(Dashboard));
        }

        // GET: Admin/Incidents
        public async Task<IActionResult> Incidents()
        {
            var incidents = await _context.DisasterIncidents
                .Include(i => i.User)
                .OrderByDescending(i => i.ReportedDate)
                .ToListAsync();
            return View(incidents);
        }

        // POST: Admin/UpdateIncidentStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIncidentStatus(int id, IncidentStatus status)
        {
            var incident = await _context.DisasterIncidents.FindAsync(id);
            if (incident == null)
            {
                return NotFound();
            }

            incident.Status = status;
            _context.Update(incident);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Incident status updated to {status}!";
            return RedirectToAction(nameof(Incidents));
        }

        // GET: Admin/Donations
        public async Task<IActionResult> Donations()
        {
            var donations = await _context.Donations
                .Include(d => d.User)
                .Include(d => d.Category)
                .OrderByDescending(d => d.DonationDate)
                .ToListAsync();
            return View(donations);
        }

        // POST: Admin/UpdateDonationStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDonationStatus(int id, DonationStatus status)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
            {
                return NotFound();
            }

            donation.Status = status;
            _context.Update(donation);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Donation status updated to {status}!";
            return RedirectToAction(nameof(Donations));
        }
    }

    // ViewModel for Admin Dashboard
    public class AdminDashboardViewModel
    {
        public List<Volunteer> PendingVolunteers { get; set; } = new();
        public int TotalVolunteers { get; set; }
        public int ApprovedVolunteers { get; set; }
        public int PendingVolunteersCount { get; set; }
        public int TotalIncidents { get; set; }
        public int ActiveIncidents { get; set; }
        public int TotalDonations { get; set; }
        public int PendingDonations { get; set; }
        public int TotalTasks { get; set; }
        public int OpenTasks { get; set; }
    }

    // ViewModel for Admin Registration
    public class AdminRegisterViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
